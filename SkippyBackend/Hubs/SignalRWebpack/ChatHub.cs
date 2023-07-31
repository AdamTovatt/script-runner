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
using SkippyBackend.Models;

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

        public static OpenAiApi OpenAi
        {
            get
            {
                if (openAi == null) openAi = new OpenAiApi(EnvironmentHelper.GetOpenAiApiKey());
                return openAi;
            }
        }
        private static OpenAiApi? openAi;

        public static DirectoryCodeProvider DefaultScriptDirectory
        {
            get
            {
                if (defaultScriptDirectory == null) defaultScriptDirectory = DirectoryCodeProvider.CreateFromRelativePath("Scripts");
                return defaultScriptDirectory;
            }
        }
        private static DirectoryCodeProvider? defaultScriptDirectory;

        public static FunctionScriptLookup FunctionLookup
        {
            get
            {
                if (functionLookup == null) functionLookup = new FunctionScriptLookup(DefaultScriptDirectory);
                return functionLookup;
            }
        }
        private static FunctionScriptLookup? functionLookup;

        public ClientData CurrentClientData { get { return clientDataObjects[Context.ConnectionId]; } }

        private static Dictionary<string, ClientData> clientDataObjects = new Dictionary<string, ClientData>();
        private bool hasLoadedAdditionalReferences = false;

        public ChatHub()
        {
            if (!hasLoadedAdditionalReferences)
            {
                ReferenceProvider.Instance.AdditionalReferencesProvider = new DirectoryAdditionalReferencesProvider(); // set up additional references provider
                ReferenceProvider.Instance.LoadAdditionalReferences();
                hasLoadedAdditionalReferences = true;
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            ChatConfiguration chatConfiguration = new ChatConfiguration();
            Conversation conversation = new Conversation(Model.Gpt35Turbo16k, 15000);

            // aquire the functions (compile them if needed)
            try
            {
                Task.Run(async () =>
                {
                    if (await FunctionLookup.LoadFunctionsAsync() is List<string> errors && errors != null)
                    {
                        errors.ForEach(error => DisplayMessage(error, chatConfiguration.Colors["Error"], 0));
                        return;
                    }
                }).Wait();
            }
            catch(Exception exception)
            {
                DisplayMessage(exception.Message, chatConfiguration.Colors["Error"], 0);
            }

            conversation.SetFunctions(FunctionLookup.GetFunctions());
            conversation.AddSystemMessage(startPrompt);

            ClientData clientData = new ClientData(chatConfiguration, conversation);
            clientDataObjects.Add(Context.ConnectionId, clientData);
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
            DisplayMessage(message, CurrentClientData.ChatConfiguration.Colors["Accent2"], 1);

            CurrentClientData.Conversation.AddUserMessage(message);

            await CompleteAsync();
        }

        public async Task CompleteAsync()
        {
            CompletionResult result = await OpenAi.CompleteAsync(CurrentClientData.Conversation);

            foreach (Choice choice in result.Choices)
            {
                if (choice.FinishReason == FinishReason.FunctionCall)
                {
                    FunctionCall? functionCall = choice.Message.FunctionCall;

                    if (functionCall == null)
                        throw new Exception("Badly formatted answer from OpenAi. It said there would be a function call but the function was missing");

                    if (FunctionLookup.TryGetCompiledScriptContainer(functionCall.Name, out ICompiledScriptContainer scriptContainer))
                    {
                        DisplayMessage($"(function call: {functionCall.Name})", CurrentClientData.ChatConfiguration.Colors["Success"], 0);

                        CompiledScript compiledScript = scriptContainer.GetCompiledScript(new ScriptContext());
                        object? returnValue = compiledScript.Run(functionCall.Arguments);

                        CurrentClientData.Conversation.AddSystemMessage($"Function call returned: {ReturnValueConverter.GetStringFromObject(returnValue)}");

                        await CompleteAsync();
                    }
                }
                else
                {
                    DisplayMessage(choice.Message.Content, CurrentClientData.ChatConfiguration.Colors["Accent1"], -1);
                }
            }
        }
    }
}
