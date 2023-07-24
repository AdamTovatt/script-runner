using OpenAi;
using OpenAi.Models.Completion;
using ScriptConverter;
using ScriptRunner;
using ScriptRunner.Helpers;
using ScriptRunner.Models;
using ScriptRunner.Providers;
using System.Drawing;

namespace Console
{
    public class ConsoleScriptRunner
    {
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

            Conversation conversation = new Conversation();
            CompletionParameter parameter = conversation.CreateCompletionParameter(Model.Default);

            string startPrompt = "You are a helpful assistant that will help the user in any way possible. " +
                                 "At your disposal you have a list of functions that you can call to help the user if it seems like the user needs it. " +
                                 "If a function needs to be called, make sure that you aquire the required parameters for the function. " +
                                 "You can ask the user for the parameters. " + 
                                 "If a users asks you to create a new script you can use the function for getting all available scripts. " +
                                 "You can then read one of the scripts (or more than one if neccessary) to learn what a script looks like. " +
                                 "When you have read an already existing script you can then create a new script following the same principles that you learned. " + 
                                 "You will see this in the script that you read for inspiration, but for example, don't forget to add the xml comments. ";

            parameter.AddSystemMessage(startPrompt);

            DirectoryScriptProvider directory = DirectoryScriptProvider.CreateFromRelativePath("scripts");
            List<ScriptCode> scripts = await directory.GetAllScriptsAsync();
            Dictionary<string, ScriptCompileResult> functions = new Dictionary<string, ScriptCompileResult>();

            foreach (ScriptCode script in scripts) // setup functions
            {
                ScriptCompileResult compileResult = script.Compile();

                if (compileResult.Errors != null)
                {
                    compileResult.Errors.ForEach(x =>
                    {
                        System.Console.WriteLine(x);
                    });

                    return;
                }

                if (compileResult.CompiledAssembly != null)
                {
                    Function function = OpenAiScriptConverter.GetAsFunction(compileResult);
                    parameter.AddFunction(function);
                    functions.Add(function.Name, compileResult);
                }
            }

            bool shouldTakeUserInput = true;
            while (true)
            {
                string? userMessage = null;
                if (shouldTakeUserInput)
                {
                    Print("You: ", ConsoleColor.Green, false);
                    userMessage = System.Console.ReadLine();

                    if (string.IsNullOrEmpty(userMessage)) continue;

                    parameter.AddUserMessage(userMessage);
                }

                CompletionResult result = await openAi.CompleteAsync(parameter);
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
                        {
                            Print("(Was supposed to call function but function call was missing)", ConsoleColor.Red);
                            continue;
                        }

                        if (functions.TryGetValue(functionCall.Name, out ScriptCompileResult? compileResult))
                        {
                            Print($"(function call: {functionCall.Name})", ConsoleColor.Cyan);

                            CompiledScript compiledScript = compileResult.GetScript(new ScriptContext());
                            object? returnValue = compiledScript.Run(functionCall.Arguments);

                            string returnValueAsString = ReturnValueConverter.GetStringFromObject(returnValue);

                            parameter.AddSystemMessage($"Function call returned: {returnValueAsString}");
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