using OpenAi.Models.Completion;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ScriptRunner.OpenAi.Models.Completion;
using ScriptRunner.OpenAi.Models.Input.Types;
using ScriptRunner.OpenAi.Models.Files;

namespace ScriptRunner.OpenAi
{
    public class OpenAiApi
    {
        public static string BaseUrl = "https://api.openai.com/v1/";

        private string apiKey;
        private HttpClient client;

        public OpenAiApi(string apiKey, HttpClient client)
        {
            this.apiKey = apiKey;
            this.client = client;
        }

        public OpenAiApi(string apiKey)
        {
            this.apiKey = apiKey;
            client = new HttpClient();
        }

        /// <summary>
        /// Will complete in a conversation. Will use the inner most child conversations if there are any
        /// </summary>
        /// <param name="conversation">The conversation to complete</param>
        /// <param name="addResultToConversation">Wether or not the result of the completion should be added to the conversation straight away. The default is true and this is recommended since it will make sure that the result is added to the right conversation then</param>
        /// <param name="allowedFailCount">How many times the completion is allowed to fail before it will throw an exception. The default is 2. This means that if there is an error from open ai it will automatically try again</param>
        /// <returns></returns>
        public async Task<CompletionResult> CompleteAsync(Conversation conversation, bool addResultToConversation = true, int allowedFailCount = 2)
        {
            int failCount = 0;

            try
            {
                CompletionResult result = await CompleteAsync(conversation.CreateCompletionParameter());

                if (addResultToConversation)
                    conversation.Add(result);

                return result;
            }
            catch (Exception exception)
            {
                failCount++;
                Type exceptionType = exception.GetType();

                if ((exceptionType != typeof(HttpRequestException) && exceptionType != typeof(JsonException)) || failCount > allowedFailCount)
                    throw;

                return await CompleteAsync(conversation);
            }
        }

        /// <summary>
        /// Will complete based on a completion parameter, usually you want to use a conversation and complete on that
        /// but if you know what you are doing you can use this method directly
        /// </summary>
        /// <param name="completionParameter">The parameter to complete on</param>
        /// <returns>A completion result of the type CompletionResult</returns>
        /// <exception cref="NullReferenceException">If the deserialized completion result is null</exception>
        /// <exception cref="CompletionException">If there was something wrong with completing</exception>
        public async Task<CompletionResult> CompleteAsync(CompletionParameter completionParameter)
        {
            HttpRequestMessage request = CreateAuthenticatedRequestMessage(HttpMethod.Post, BaseUrl + "chat/completions");

            string jsonContent = completionParameter.ToJson();
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    CompletionResult? result = JsonSerializer.Deserialize<CompletionResult>(await response.Content.ReadAsStringAsync());

                    if (result == null)
                        throw new NullReferenceException($"Deserialized CompletionResult was unexpectedly null when completing based on the completion parameter {completionParameter}");

                    return result;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    Thread.Sleep(1000);
                    return await CompleteAsync(completionParameter);
                }

                throw new CompletionException($"({response.StatusCode}) Error when completing based on the parameter {completionParameter}.\n{await response.Content.ReadAsStringAsync()}", response);
            }
            catch (TimeoutException)
            {
                throw new CompletionException("Error when creating an answer, the connection to OpenAi timed out.", null);
            }
        }

        /// <summary>
        /// Will upload a file
        /// </summary>
        /// <param name="fileData">The string content of the file</param>
        /// <param name="purpose">The purpose of the file, defaults to "fine-tune"</param>
        /// <returns>A upload file result, check that the Error property is null if you want to ensure a success</returns>
        public async Task<UploadFileResult> UploadFileAsync(string fileData, string purpose = "fine-tune")
        {
            HttpRequestMessage request = CreateAuthenticatedRequestMessage(HttpMethod.Post, BaseUrl + "files");

            request.Content = new MultipartFormDataContent
            {
                { new ByteArrayContent(Encoding.UTF8.GetBytes(fileData)), "file", "fileData.txt" },
                { new StringContent(purpose), "purpose" }
            };

            HttpResponseMessage response = await client.SendAsync(request);

            return UploadFileResult.FromJson(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Will delete a file
        /// </summary>
        /// <param name="fileId">The id of the file to delete</param>
        /// <returns>A delete file result</returns>
        public async Task<DeleteFileResult> DeleteFileAsync(string fileId)
        {
            HttpRequestMessage request = CreateAuthenticatedRequestMessage(HttpMethod.Delete, BaseUrl + $"files/{fileId}");

            return DeleteFileResult.FromJson(await (await client.SendAsync(request)).Content.ReadAsStringAsync());
        }

        private HttpRequestMessage CreateAuthenticatedRequestMessage(HttpMethod method, string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            return request;
        }

        public async Task<string> GetSingleAnswerAsync(string prompt, string answer, string? completionModel = null)
        {
            string model = Model.Default;

            if (completionModel != null)
                model = completionModel;

            CompletionParameter completionParameter = new CompletionParameter(model);
            completionParameter.AddSystemMessage(prompt);
            completionParameter.AddUserMessage(answer);

            CompletionResult result = await CompleteAsync(completionParameter);

            return result.Choices.First().Message.Content;
        }

        /// <summary>
        /// Will extract a specific type of value from a string message. If the chosen type is string it will instead extract the item that that string is talking about
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ExtractionResult<T>> ExtractAsync<T>(string message) where T : IInputType
        {
            Type type = typeof(T);

            if (type == typeof(BoolInputType))
                return (await ExtractBoolAsync(message) as ExtractionResult<T>)!;
            else if (type == typeof(IntegerInputType))
                return (await ExtractIntAsync(message) as ExtractionResult<T>)!;
            else if (type == typeof(DecimalInputType))
                return (await ExtractDecimalAsync(message) as ExtractionResult<T>)!;
            else if (type == typeof(StringInputType))
                return (await ExtractStringAsync(message) as ExtractionResult<T>)!;

            throw new InvalidOperationException($"Extracting the type {type} is not supported. ");
        }

        /// <summary>
        /// Extract the subject of the string
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<ExtractionResult<StringInputType>> ExtractStringAsync(string message)
        {
            CompletionParameter completionParameter = new CompletionParameter(Model.Default);
            completionParameter.AddSystemMessage("You will be given a message that is talking about a specific item. Answer with only with that item! Answer with (invalid) if not possible. Here are some examples: ");
            completionParameter.AddUserMessage("I want to insure my hairdryer");
            completionParameter.AddAssistantMessage("hairdryer");
            completionParameter.AddUserMessage("it is my dog");
            completionParameter.AddAssistantMessage("dog");
            completionParameter.AddUserMessage("Some random tool in my shed");
            completionParameter.AddAssistantMessage("tool");
            completionParameter.AddUserMessage("019232");
            completionParameter.AddAssistantMessage("(invalid)");
            completionParameter.AddUserMessage("this can't really be turned into an item");
            completionParameter.AddAssistantMessage("(invalid)");

            completionParameter.AddSystemMessage("Now the real message from the user. Answer only with the item that the message is about or (invalid) if not possible. ");

            completionParameter.AddUserMessage(message);

            CompletionResult completionResult = await CompleteAsync(completionParameter);

            string completionResultString = completionResult.Choices.First().Message.Content;

            ExtractionResult<StringInputType> result = new ExtractionResult<StringInputType>(completionResultString);

            return result;
        }

        private async Task<ExtractionResult<DecimalInputType>> ExtractDecimalAsync(string message)
        {
            ExtractionResult<DecimalInputType> result = new ExtractionResult<DecimalInputType>(message);

            if (!result.Valid)
                result = new ExtractionResult<DecimalInputType>(await GetSingleAnswerAsync("You will be given a string with a number. Extract the number and answer it only that! It is important that your answer contains ONLY the number, nothing else. If it's not possible answer with (invalid)", message));

            return result;
        }

        private async Task<ExtractionResult<BoolInputType>> ExtractBoolAsync(string message)
        {
            ExtractionResult<BoolInputType> result = new ExtractionResult<BoolInputType>(message);

            if (!result.Valid)
                result = new ExtractionResult<BoolInputType>(await GetSingleAnswerAsync("You will be given a string that you will convert to a boolean. You can think of anything that could mean no as false and anything that could mean yes as true. Answer with only true or false! Answer with (invalid) if not possible", message));

            return result;
        }

        private async Task<ExtractionResult<IntegerInputType>> ExtractIntAsync(string message)
        {
            ExtractionResult<IntegerInputType> result = new ExtractionResult<IntegerInputType>(message);

            if (!result.Valid)
                result = new ExtractionResult<IntegerInputType>(await GetSingleAnswerAsync("You will be given a string with a number and some other things. Answer with only the number! Answer with (invalid) if not possible", message));

            return result;
        }
    }
}