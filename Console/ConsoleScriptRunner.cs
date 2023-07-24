using OpenAi;
using OpenAi.Models.Completion;
using ScriptConverter;
using ScriptRunner;
using ScriptRunner.Models;
using ScriptRunner.Providers;

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

            Conversation conversation = new Conversation();
            CompletionParameter parameter = conversation.CreateCompletionParameter(Model.Default);

            string startPrompt = "You are a helpful assistant that will help the user in any way possible. " +
                                 "At your disposal you have a list of functions that you can call to help the user if it seems like the user needs it. " +
                                 "If a function needs to be called, make sure that you aquire the required parameters for the function. " +
                                 "You can ask the user for the parameters. ";

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
                    System.Console.Write("You: ");
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
                            System.Console.WriteLine("(Was supposed to call function but function call was missing");
                            continue;
                        }

                        if (functions.TryGetValue(functionCall.Name, out ScriptCompileResult? compileResult))
                        {
                            CompiledScript compiledScript = compileResult.GetScript(new ScriptContext());
                            object? returnValue = compiledScript.Run(functionCall.Arguments);

                            string returnValueAsString = returnValue?.ToString() ?? "null";

                            parameter.AddSystemMessage($"Function call returned: {returnValueAsString}");
                            shouldTakeUserInput = false;
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Bot: " + choice.Message.Content);
                    }
                }
            }
        }
    }
}