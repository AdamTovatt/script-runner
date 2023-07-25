using OpenAi;
using OpenAi.Models.Completion;
using ScriptConverter;
using ScriptRunner;
using ScriptRunner.Helpers;
using ScriptRunner.Models;
using ScriptRunner.Providers;

namespace Console
{
    public class ConsoleScriptRunner
    {
        private const string startPrompt = "You are a helpful assistant that will help the user in any way possible. " +
                                        "At your disposal you have a list of functions that you can call to help the user if it seems like the user needs it. " +
                                        "If a function needs to be called, make sure that you aquire the required parameters for the function. " +
                                        "You can ask the user for the parameters. " +
                                        "Always use the correct script structure when creating new scripts. " +
                                        "If a user asks you to create a new script you should first load the DivideScript.cs and use that for inspiration for the new script." +
                                        "Don't use functions that doesn't exist. ";

        static async Task Main(string[] args)
        {
            try
            {
                await HandleConversation();
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                System.Console.WriteLine(fileNotFoundException.Message);
            }

            System.Console.WriteLine("Application finished, press enter to exit...");
            System.Console.ReadLine();
        }
        
        static async Task HandleConversation()
        {
            OpenAiApi openAi = new OpenAiApi(EnvironmentHelper.GetOpenAiApiKey());

            ReferenceProvider.Instance.AdditionalReferencesProvider = new DirectoryAdditionalReferencesProvider(); // set up additional references provider
            ReferenceProvider.Instance.LoadAdditionalReferences();

            DirectoryCodeProvider directory = DirectoryCodeProvider.CreateFromRelativePath("scripts");
            FunctionScriptLookup functionLookup = new FunctionScriptLookup(directory);

            if (await functionLookup.LoadFunctionsAsync() is List<string> errors && errors != null)
            {
                errors.ForEach(error => Print(error, ConsoleColor.Red));
                return;
            }

            Conversation conversation = new Conversation(Model.Gpt35Turbo16k, 15000);
            conversation.SetFunctions(functionLookup.GetFunctions());
            conversation.AddSystemMessage(startPrompt);

            bool shouldTakeUserInput = true;
            while (true)
            {
                string? userMessage = null;
                if (shouldTakeUserInput)
                {
                    Print("You: ", ConsoleColor.Green, false);
                    userMessage = System.Console.ReadLine();

                    if (string.IsNullOrEmpty(userMessage)) continue;

                    conversation.AddUserMessage(userMessage);
                }

                CompletionResult result = await openAi.CompleteAsync(conversation);
                shouldTakeUserInput = true;

                if (result == null)
                    break;

                conversation.Add(result);

                foreach (Choice choice in result.Choices)
                {
                    if (choice.FinishReason == FinishReason.FunctionCall)
                    {
                        FunctionCall? functionCall = choice.Message.FunctionCall;

                        if (functionCall == null)
                            throw new Exception("Badly formatted answer from OpenAi. It said there would be a function call but the function was missing");

                        if (functionLookup.TryGetCompileResult(functionCall.Name, out ScriptCompileResult compileResult))
                        {
                            Print($"(function call: {functionCall.Name})", ConsoleColor.Cyan);

                            CompiledScript compiledScript = compileResult.GetScript(new ConsoleRunnerContext(functionLookup));
                            object? returnValue = compiledScript.Run(functionCall.Arguments);

                            conversation.AddSystemMessage($"Function call returned: {ReturnValueConverter.GetStringFromObject(returnValue)}");
                            shouldTakeUserInput = false;
                        }
                    }
                    else
                    {
                        Print($"Bot: {choice.Message.Content}", ConsoleColor.Blue);
                    }
                }
            }
        }

        private static void Print(string message, ConsoleColor color, bool includeLineBreak = true)
        {
            System.Console.ForegroundColor = color;

            if (includeLineBreak)
                System.Console.WriteLine(message);
            else
                System.Console.Write(message);
        }
    }
}