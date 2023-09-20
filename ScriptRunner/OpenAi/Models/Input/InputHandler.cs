using ScriptRunner.OpenAi.Helpers;
using ScriptRunner.OpenAi.Models.Completion;
using ScriptRunner.OpenAi.Models.Input.Types;
using System;
using System.Buffers.Text;
using System.Text;

namespace ScriptRunner.OpenAi.Models.Input
{
    public class InputHandler
    {
        /// <summary>
        /// The duration to wait between each check for new input from the user (in milliseconds). Default is 500
        /// </summary>
        public static int InputStreamRefreshWaitDuration = 500;

        /// <summary>
        /// The conversation that this input handler is attached to
        /// </summary>
        public Conversation Conversation { get; set; }

        private object userInputStreamLock = new object(); // used to lock the input stream when it is being used from one thread
        private MemoryStream? userInputStream; // the input stream that the input of the user will be written to

        /// <summary>
        /// Will contain information about the currently requested input
        /// </summary>
        public InputInfo? CurrentInputInfo { get; private set; }

        public InputHandler(Conversation conversation)
        {
            Conversation = conversation;
        }

        public async Task<T?> GetAsync<T>(string inputMessage, string subType)
        {
            return await GetAsync<T>(new InputInfo(typeof(T), inputMessage, subType, null));
        }

        public async Task<T?> GetAsync<T>(string inputMessage, List<InputChoice> choices)
        {
            return await GetAsync<T>(new InputInfo(typeof(T), inputMessage, null, choices));
        }

        /// <summary>
        /// Be careful so that you don't use this method with a non nullable type when you don't really want to, it will give the default value of that type which is not always null and might lead to unexpected bugs for types that have a default value that is not null (value types)
        /// </summary>
        public async Task<T?> GetAsync<T>(string inputMessage, string? subType = null, List<InputChoice>? choices = null)
        {
            return await GetAsync<T>(new InputInfo(typeof(T), inputMessage, subType, choices));
        }

        /// <summary>
        /// Will get input async
        /// </summary>
        /// <typeparam name="T">The type of input value to get</typeparam>
        /// <param name="inputMessage">The prompt to the user</param>
        /// <param name="ensureNotNull">If we should ensure that the input is not null by retrying untill it is not null or max attempts is reached</param>
        /// <param name="retryPromptMessage">The prompt if the input is faild and retried</param>
        /// <param name="maxAttemptCount">The max attempts to try to ensure it is not null</param>
        /// <param name="subType">The sub type of input</param>
        /// <param name="choices">The choices that should be given to the user</param>
        /// <param name="customExtractor">If a custom extractor should be used to extract the value it can be provided here</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InputException"></exception>
        public async Task<T?> GetAsync<T>(string inputMessage, bool ensureNotNull, string? retryPromptMessage = null, int? maxAttemptCount = null, string? subType = null, List<InputChoice>? choices = null, Func<string, object?>? customExtractor = null)
        {
            if (!typeof(T).IsNullable())
                throw new ArgumentException("The type T must be nullable. When ensuring that input is not null. ");

            string messageToUse = inputMessage;

            int attempts = 0;
            while (maxAttemptCount == null || attempts < maxAttemptCount)
            {
                T? result = await GetAsync<T?>(new InputInfo(typeof(T), messageToUse, subType, choices), customExtractor);

                if (ensureNotNull && result != null && typeof(T) == typeof(string)) // for strings we check for a mentioned item if ensure not null is set to true
                {
                    ExtractionResult<StringInputType> extractionResult = await Conversation.OpenAi.ExtractAsync<StringInputType>((string)(object)result!);

                    if (extractionResult.Valid)
                        result = (T?)(object?)extractionResult.ExtractedValue?.Value;
                    else
                        result = default;
                }

                if (result != null || !ensureNotNull)
                    return result;

                if (retryPromptMessage != null)
                    messageToUse = retryPromptMessage;

                attempts++;
            }

            throw new InputException();
        }

        /// <summary>
        /// Will get input from the frontend
        /// </summary>
        /// <typeparam name="T">The type of the input</typeparam>
        /// <param name="inputInfo">Contains for example a list of choices that an end user can chose between as well as the prompt to show</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Will be thrown if the conversation that used to take input doesn't have a communicator that has had it's event (OnWantsInput) subscribed to. </exception>
        /// <exception cref="InvalidDataException"></exception>
        public async Task<T?> GetAsync<T>(InputInfo inputInfo, Func<string, object?>? customExtractor = null)
        {
            byte[]? result = null;

            using (userInputStream = new MemoryStream())
            {
                InputInfo finalizedInputInfo = FinalizeInputInfo(typeof(T), inputInfo);

                CurrentInputInfo = finalizedInputInfo;

                if (!Conversation.Communicator.InvokeOnWantsInput(this, finalizedInputInfo)) // invoke the event that the communicator needs to handle so that an input request can be sent to frontend
                    throw new InvalidOperationException("An attempt to take user input from a conversation where the communicator hasn't been correctly subscribed to was made. Make sure to subscribe to and handle the OnWantsInput event. ");

                while (true)
                {
                    lock (userInputStreamLock)
                    {
                        if (userInputStream.Length > 0)
                            result = userInputStream.ToArray();
                    }

                    if (result != null) break;

                    await Task.Delay(InputStreamRefreshWaitDuration);
                }
            }

            CurrentInputInfo = null;
            userInputStream = null;

            try
            {
                object? resultObject = await RefineUserInputResultAsync(typeof(T), result, inputInfo, customExtractor);

                if (resultObject == null) return default;
                return (T)resultObject;
            }
            catch (Exception exception)
            {
                throw new InvalidDataException($"An error occured when converting the input from the user (length: {result.Length}) to the right type ({typeof(T)}). ", exception);
            }
        }

        private async Task<object?> RefineUserInputResultAsync(Type type, byte[] result, InputInfo inputInfo, Func<string, object?>? customExtractor = null)
        {
            if (type == typeof(byte[])) // handle byte[]
                return Convert.FromBase64String(Encoding.UTF8.GetString(result));
            else // all other types will be handled through the string representation of the input
            {
                string stringValue = Encoding.UTF8.GetString(result); // get the string input. This is can probably be seen as the text of the message in a way

                if (type == typeof(string)) // don't need to do anything here
                    return stringValue;

                if (type.IsAnyOf(typeof(decimal), typeof(double), typeof(float), typeof(decimal?), typeof(double?), typeof(float?))) // handle decimal types
                {
                    decimal? decimalValue;

                    if (customExtractor == null)
                        decimalValue = (await Conversation.OpenAi.ExtractAsync<DecimalInputType>(stringValue)).ExtractedValue?.Value;
                    else
                        return customExtractor(stringValue);

                    if (type.IsAnyOf(typeof(double), typeof(double?)))
                        return (double?)decimalValue;
                    if (type.IsAnyOf(typeof(float), typeof(float?)))
                        return (float?)decimalValue;

                    return decimalValue;
                }

                if (type.IsAnyOf(typeof(int), typeof(long), typeof(short), typeof(byte), typeof(int?), typeof(long?), typeof(short?), typeof(byte?))) // handle integer types
                {
                    long? longValue = null;

                    if (inputInfo.HasChoices) // if there are choices, let's first try to use them to get the value
                    {
                        InputChoice? inputChoice = inputInfo.GetInputChoiceByMessage(stringValue);
                        if (inputChoice != null && inputChoice.Value != null && inputChoice.Value.ToString() != null)
                            longValue = (await Conversation.OpenAi.ExtractAsync<IntegerInputType>(inputChoice.Value.ToString()!)).ExtractedValue?.Value;
                    }

                    if (longValue == null && customExtractor != null)
                        return customExtractor(stringValue);

                    if (longValue == null) // if it's still null, maybe because the user wrote something that isn't an option, then we'll use what they wrote normally
                        longValue = (await Conversation.OpenAi.ExtractAsync<IntegerInputType>(stringValue)).ExtractedValue?.Value;

                    if (type.IsAnyOf(typeof(int), typeof(int?)))
                        return (int?)longValue;
                    if (type.IsAnyOf(typeof(short), typeof(short?)))
                        return (short?)longValue;
                    if (type.IsAnyOf(typeof(byte), typeof(byte?)))
                        return (byte?)longValue;

                    return longValue;
                }

                if (type.IsAnyOf(typeof(bool), typeof(bool?))) // handle bool
                {
                    bool? boolValue = null;

                    if (inputInfo.HasChoices) // if it has choices we'll try to use them to get the value
                    {
                        InputChoice? inputChoice = inputInfo.GetInputChoiceByMessage(stringValue);
                        if (inputChoice != null)
                        {
                            boolValue = (bool?)inputChoice.Value;
                        }
                    }

                    if (boolValue == null) // if it's still null, maybe because the user wrote something that isn't an option, then we'll use what they wrote normally
                        boolValue = (await Conversation.OpenAi.ExtractAsync<BoolInputType>(stringValue)).ExtractedValue?.Value;

                    return boolValue;
                }

                if (type == typeof(InputChoice)) // if the type is an input choice we want to return the choice that matches the input string
                {
                    if (inputInfo.Choices == null || inputInfo.Choices.Count == 0) // this should not happen
                        throw new InvalidOperationException("An attempt to get an input choice from the user was made, but no choices were specified. ");

                    return inputInfo.GetInputChoiceByMessage(stringValue);
                }

                return Convert.ChangeType(result, type);
            }
        }

        public void AddResponse(string response)
        {
            AddResponse(Encoding.UTF8.GetBytes(response));
        }

        public void AddResponse(byte[] bytes)
        {
            if (userInputStream == null)
                throw new InvalidOperationException("The user input stream is null, like no input is being taken, yet an attempt to write a user input was made. ");

            lock (userInputStreamLock)
            {
                userInputStream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Will automatically add the right default options for bool inputs if no options exist
        /// </summary>
        private InputInfo FinalizeInputInfo(Type type, InputInfo inputInfo)
        {
            if (type.IsAnyOf(typeof(bool), typeof(bool?))) // if it is a bool we want to make sure the right choices exist
            {
                if (inputInfo.Choices == null || inputInfo.Choices.Count == 0)
                    inputInfo.AddChoices(new InputChoice(true, "Yes"), new InputChoice(false, "No"));
            }

            return inputInfo;
        }
    }
}
