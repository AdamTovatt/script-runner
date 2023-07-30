using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using OpenAi;
using OpenAi.Models.Completion;
using ScriptConverter;
using ScriptRunner.Helpers;
using ScriptRunner.Models;
using ScriptRunner;
using ScriptRunner.Providers;
using SkippyBackend.FrontEndModels;
using SkippyBackend.Helpers;

namespace SkippyBackend.Hubs.SignalRWebpack
{
    [EnableCors]
    public class ChatHub : Hub
    {
        private const string startPrompt = "You are a helpful assistant that will help the user in any way possible. " +
                                "At your disposal you have a list of functions that you can call to help the user if it seems like the user needs it. " +
                                "If a function needs to be called, make sure that you aquire the required parameters for the function. " +
                                "You can ask the user for the parameters. " +
                                "Always use the correct script structure when creating new scripts. " +
                                "If a user asks you to create a new script you should first load the DivideScript.cs and use that for inspiration for the new script." +
                                "Don't use functions that doesn't exist. ";

        public ChatConfiguration ChatConfiguration { get; set; }
        public Conversation Conversation { get; set; }
        public OpenAiApi OpenAi { get; set; }
        public FunctionScriptLookup FunctionLookup { get; set; }
        public DirectoryCodeProvider Directory { get; set; }

        public ChatHub()
        {
            ChatConfiguration = new ChatConfiguration();

            OpenAi = new OpenAiApi(EnvironmentHelper.GetOpenAiApiKey());

            ReferenceProvider.Instance.AdditionalReferencesProvider = new DirectoryAdditionalReferencesProvider(); // set up additional references provider
            ReferenceProvider.Instance.LoadAdditionalReferences();

            Directory = DirectoryCodeProvider.CreateFromRelativePath("Scripts");
            FunctionLookup = new FunctionScriptLookup(Directory);

            Conversation = new Conversation(Model.Gpt35Turbo16k, 15000);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            Task.Run(async () =>
            {
                if (await FunctionLookup.LoadFunctionsAsync() is List<string> errors && errors != null)
                {
                    errors.ForEach(error => DisplayMessage(error, ChatConfiguration.Colors["Error"], 0));
                    return;
                }
            }).Wait();

            Conversation.SetFunctions(FunctionLookup.GetFunctions());
            Conversation.AddSystemMessage(startPrompt);
        }

        private void DisplayMessage(DisplayMessage displayMessage)
        {
            Clients.All.SendAsync("messageReceived", displayMessage);
        }

        private void DisplayMessage(string text, string color, int align)
        {
            DisplayMessage displayMessage = new DisplayMessage(text, color, align);

            DisplayMessage(displayMessage);
        }

        /// RPC
        public async Task NewMessage(string message)
        {
            DisplayMessage(message, ChatConfiguration.Colors["Accent2"], 1);

            Conversation.AddUserMessage(message);

            CompletionResult result = await OpenAi.CompleteAsync(Conversation);

            Conversation.Add(result);

            foreach (Choice choice in result.Choices)
            {
                if (choice.FinishReason == FinishReason.FunctionCall)
                {
                    FunctionCall? functionCall = choice.Message.FunctionCall;

                    if (functionCall == null)
                        throw new Exception("Badly formatted answer from OpenAi. It said there would be a function call but the function was missing");

                    if (FunctionLookup.TryGetCompileResult(functionCall.Name, out ScriptCompileResult compileResult))
                    {
                        DisplayMessage($"(function call: {functionCall.Name})", ChatConfiguration.Colors["Success"], -1);

                        CompiledScript compiledScript = compileResult.GetScript(new ScriptContext());
                        object? returnValue = compiledScript.Run(functionCall.Arguments);

                        Conversation.AddSystemMessage($"Function call returned: {ReturnValueConverter.GetStringFromObject(returnValue)}");
                    }
                }
                else
                {
                    DisplayMessage(choice.Message.Content, ChatConfiguration.Colors["Accent1"], -1);
                }
            }
        }
    }
}
