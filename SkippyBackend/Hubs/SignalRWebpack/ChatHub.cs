﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using ScriptRunner.Helpers;
using ScriptRunner.Providers;
using SkippyBackend.FrontEndModels;
using SkippyBackend.Helpers;
using SkippyBackend.Models;
using SkippyBackend.PrecompiledScripts;
using ScriptRunner.ScriptConvertion;
using ScriptRunner.OpenAi;
using ScriptRunner.OpenAi.Models.Completion;
using ScriptRunner.OpenAi.Models.Input;
using Microsoft.AspNetCore.Authorization;
using ScriptRunner.Models;

namespace SkippyBackend.Hubs.SignalRWebpack
{
    [Authorize("admin")]
    [EnableCors]
    public class ChatHub : Hub
    {
        private const string startPrompt = "You are a helpful assistant called Skippy but you are also a very laid back and chill guy. Answer with things like \"Hey, what's up?\" and \"Yeah, sure\". " +
                                "At your disposal you have a list of functions that you can call to help the user if it seems like the user needs it. " +
                                "If a function needs to be called, make sure that you aquire the required parameters for the function. Don't ever call a function without being sure about the parameters, ask the user for confirmation if you're unsure. " +
                                "You can ask the user for the parameters. " +
                                "Always use the correct script structure when creating new scripts. " +
                                "If a user asks you to create a new script you should first load the DivideScript.cs and use that for inspiration for the new script." +
                                "Don't use functions that doesn't exist. " +
                                "Also, please have a very laid back persona. Maybe say things like \"yeah, sure, why not\" if the user asks you if you can do something. Don't overdo it but have a very chill vibe personality. ";

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
                if (functionLookup == null) functionLookup =
                        new FunctionScriptLookup(
                            //DefaultScriptDirectory,
                            new PreCompiledScriptProvider(
                                typeof(GetAvailableFunctionsScript),
                                typeof(CreateSingleItemScript),
                                typeof(ChangeAssistantColorScript),
                                typeof(ChangeUserColorScript),
                                typeof(UploadSvgToCloudinaryScript),
                                typeof(GetFlightsScript),
                                typeof(RunLinuxCommandScript),
                                typeof(SendTextFileScript)
                                )
                            );
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
            Conversation conversation = new Conversation(OpenAi, Model.Gpt35Turbo16k, 15000);
            FunctionLookup.SetClaimsPrincipal(Context.User);

            // aquire the functions (compile them if needed)
            try
            {
                if (await FunctionLookup.LoadFunctionsAsync() is List<string> errors && errors != null)
                {
                    errors.ForEach(error => DisplayMessage(error, chatConfiguration.Colors["Error"], 0));
                    return;
                }
            }
            catch (Exception exception)
            {
                DisplayMessage(exception.Message, chatConfiguration.Colors["Error"], 0);
            }

            conversation.SetFunctionLookup(FunctionLookup);
            conversation.AddSystemMessage(startPrompt);
            conversation.Add(new DirectoryWorkflowProvider("workflows"));

            ClientData clientData = new ClientData(chatConfiguration, conversation);
            clientDataObjects.Add(Context.ConnectionId, clientData);
        }

        private void DisplayMessage(DisplayMessage displayMessage)
        {
            Clients.Client(Context.ConnectionId).SendAsync("messageReceived", displayMessage);
        }

        private void DisplayMessage(string text, string color, int align)
        {
            DisplayMessage displayMessage = new DisplayMessage(text, color, align);

            DisplayMessage(displayMessage);
        }

        private void ShowInfoMessage(InfoMessage infoMessage)
        {
            Clients.Client(Context.ConnectionId).SendAsync("showInfoMessage", infoMessage);
        }

        private void ShowTypingAnimation()
        {
            Clients.Client(Context.ConnectionId).SendAsync("showTypingAnimation");
        }

        // RPC
        public async Task NewMessage(string message)
        {
            DisplayMessage(message, CurrentClientData.ChatConfiguration.Colors["Accent2"], 1);

            CurrentClientData.Conversation.ActiveConversation.AddUserMessage(message);

            await CompleteAsync();
        }

        // RPC
        public async Task SubmitInput(string input)
        {
            InputInfo? inputInfo = CurrentClientData.Conversation.Input.CurrentInputInfo;

            if (inputInfo == null) throw new NullReferenceException("Input info is null when it should not be");

            if (inputInfo.SubType == SubTypes.Image)
                DisplayMessage(new DisplayMessage(input, CurrentClientData.ChatConfiguration.Colors["Accent2"], 1, ContentType.Image, "userImage.png"));
            else
                DisplayMessage(input, CurrentClientData.ChatConfiguration.Colors["Accent2"], 1);

            await Task.CompletedTask;
            ShowTypingAnimation();
            CurrentClientData.Conversation.ActiveConversation.Input.AddResponse(input);
        }

        private void ConversationWantsInput(object sender, InputInfo inputInfo)
        {
            DisplayMessage promptMessage = new DisplayMessage(inputInfo.Message, CurrentClientData.ChatConfiguration.Colors["Accent1"], -1);
            DisplayMessage userResponseInfo = new DisplayMessage("", CurrentClientData.ChatConfiguration.Colors["Accent2"], 1);
            Clients.Client(Context.ConnectionId).SendAsync("requestInput", new InputRequest(inputInfo, promptMessage, userResponseInfo));
        }

        private void ConversationSystemMessageAdded(object sender, string message)
        {
            ShowInfoMessage(new InfoMessage(message, InfoMessage.InfoMessageType.Success));
        }

        private void ConversationMessageRecieved(object sender, string message)
        {
            DisplayMessage(message, CurrentClientData.ChatConfiguration.Colors["Accent1"], -1);
        }

        private void ConversationFunctionCallWasMade(object sender, FunctionCall functionCall)
        {
            ShowInfoMessage(new InfoMessage($"(function call: {functionCall.Name})", InfoMessage.InfoMessageType.Success));
        }

        private void ConversationErrorOccured(object sender, string message)
        {
            DisplayMessage(message, CurrentClientData.ChatConfiguration.Colors["Error"], 0);
        }

        private void ConversationFileWasSent(object sender, byte[] content, ContentType fileType, string fileName)
        {
            string base64Content = Convert.ToBase64String(content);
            string color = CurrentClientData.ChatConfiguration.Colors["Accent1"];
            DisplayMessage(new DisplayMessage(base64Content, color, -1, fileType, fileName));
        }

        public async Task CompleteAsync()
        {
            Conversation conversation = CurrentClientData.Conversation;

            try
            {
                conversation.Communicator.OnCompletionMessageRecieved += ConversationMessageRecieved;
                conversation.Communicator.OnFunctionCallWasMade += ConversationFunctionCallWasMade;
                conversation.Communicator.OnErrorOccured += ConversationErrorOccured;
                conversation.Communicator.OnSystemMessageAdded += ConversationSystemMessageAdded;
                conversation.Communicator.OnWantsInput += ConversationWantsInput;
                conversation.Communicator.OnFileWasSent += ConversationFileWasSent;

                ShowTypingAnimation();

                await conversation.CompleteAsync(new SkippyContext(CurrentClientData));
            }
            catch { throw; }
            finally
            {
                conversation.Communicator.OnCompletionMessageRecieved -= ConversationMessageRecieved;
                conversation.Communicator.OnFunctionCallWasMade -= ConversationFunctionCallWasMade;
                conversation.Communicator.OnErrorOccured -= ConversationErrorOccured;
                conversation.Communicator.OnSystemMessageAdded -= ConversationSystemMessageAdded;
                conversation.Communicator.OnWantsInput -= ConversationWantsInput;
                conversation.Communicator.OnFileWasSent -= ConversationFileWasSent;
            }
        }
    }
}
