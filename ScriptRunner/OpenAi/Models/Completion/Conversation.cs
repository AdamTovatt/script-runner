using ScriptRunner.Helpers;
using ScriptRunner.Models;
using ScriptRunner.OpenAi.Helpers;
using ScriptRunner.OpenAi.Models.Input;
using ScriptRunner.Providers;
using ScriptRunner.ScriptConvertion;
using ScriptRunner.Workflows;
using ScriptRunner.Workflows.Scripts;
using System.Text.Json;

namespace ScriptRunner.OpenAi.Models.Completion
{
    public class Conversation
    {
        /// <summary>
        /// This will control if a system message should be added when a function is called, sometimes, adding a system message makes the bot
        /// start to say that it is calling a function because it sees the system messages and starts to mimmic them.
        /// This is obviously bad since then it just says that it's calling a function instead of actually doing it.
        /// This allows the system messages to be turned off, can be used to turn them off if it seems like the bot is mimmicing the system messages too much
        /// </summary>
        public static bool AddSystemMessageOnFunctionCall { get; set; } = true;

        /// <summary>
        /// Used to communicate with the frontend, has loads of usefull events
        /// </summary>
        public Communicator Communicator { get; set; }

        /// <summary>
        /// Reference to the open ai api
        /// </summary>
        public OpenAiApi OpenAi { get; set; }

        /// <summary>
        /// The messages in this conversation
        /// </summary>
        public List<Message> Messages { get; set; }

        /// <summary>
        /// The functions in this conversation
        /// </summary>
        public List<Function>? Functions { get { if (FunctionLookup == null) return null; return FunctionLookup.GetFunctions(); } }

        /// <summary>
        /// The FunctionScriptLookup to use for the functions
        /// </summary>
        public FunctionScriptLookup? FunctionLookup { get; set; }

        /// <summary>
        /// The model to use for completion
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The size of this context in tokens, if this is not null, older messages will be removed to not run out of context
        /// </summary>
        public int? TokenLimit { get; set; }

        /// <summary>
        /// Having child and parent conversations in a conversation allows us to have subconversations. Used together with ActiveConversation
        /// </summary>
        public Conversation? ParentConversation { get; set; }

        /// <summary>
        /// Having child and parent conversations in a conversation allows us to have subconversations. Used together with ActiveConversation
        /// </summary>
        public Conversation? ChildConversation { get; set; }

        /// <summary>
        /// Kind of deprecated, or this feature needs work if it's gonna work
        /// </summary>
        public Workflow? Workflow { get; set; }

        /// <summary>
        /// Also kind of deprecated, or this feature needs work if it's gonna work
        /// </summary>
        public IWorkflowProvider? WorkflowProvider { get; set; }

        /// <summary>
        /// The currently active conversation. This is necessary since a conversation can have subconversations and we want to use the one that is currently active and being used
        /// </summary>
        public Conversation ActiveConversation { get { return GetActiveConversation(); } }

        /// <summary>
        /// Used to handle input to the conversation
        /// </summary>
        public InputHandler Input { get; private set; }

        private TokenCounter tokenCounter; // used to count tokens of a message to know if we should remove the first messages to not run out of context

        public Conversation(OpenAiApi openAi, string model, int? tokenLimit = null)
        {
            OpenAi = openAi;
            TokenLimit = tokenLimit;
            Model = model;
            Messages = new List<Message>();
            tokenCounter = new TokenCounter(model);
            Communicator = new Communicator();
            Input = new InputHandler(this);
        }

        public void Add(IWorkflowProvider workflowProvider)
        {
            WorkflowProvider = workflowProvider;
        }

        public void Add(CompletionParameter parameter)
        {
            if (parameter.Messages == Messages)
                return;

            foreach (Message message in parameter.Messages)
            {
                Messages.Add(message);
            }
        }

        /// <summary>
        /// Makes sure that a the result of a completion is correctly added to the conversation
        /// </summary>
        /// <param name="result"></param>
        public void Add(CompletionResult result)
        {
            foreach (Choice choice in result.Choices)
            {
                if (choice.FinishReason != FinishReason.FunctionCall)
                    Messages.Add(choice.Message!);
                else
                {
                    string? functionArguments = null;

                    if (choice.Message != null && choice.Message.FunctionCall != null && choice.Message.FunctionCall.Arguments != null)
                        functionArguments = JsonSerializer.Serialize(choice.Message.FunctionCall.Arguments);

                    if (AddSystemMessageOnFunctionCall) // use property to determine if we should actually add a system message
                    {
                        // The next line will add a system message in the conversation that says that a function is being called
                        Messages.Add(new Message(Role.System, functionArguments == null ? "(Calling function)" : $"(Calling function, paramters: {functionArguments})"));
                    }
                }
            }

            if (TokenLimit != null && TokenLimit > 0)
            {
                while (tokenCounter.GetTokenCount(this) > TokenLimit)
                {
                    Messages.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// A completion parameter is used to send to the open ai api to get a completion result, it contains all the messages and functions
        /// </summary>
        /// <returns></returns>
        public CompletionParameter CreateCompletionParameter()
        {
            return new CompletionParameter(Model, Messages, Functions);
        }

        public void SetFunctionLookup(FunctionScriptLookup functionScriptLookup)
        {
            FunctionLookup = functionScriptLookup;
        }

        /// <summary>
        /// Will add a message to the conversation
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        /// <summary>
        /// Will add message coming from the system
        /// </summary>
        /// <param name="content">The text content of the message</param>
        public void AddSystemMessage(string content)
        {
            Messages.Add(new Message(Role.System, content));
            Communicator.InvokeOnSystemMessageAdded(this, content);
        }

        /// <summary>
        /// Will add a message coming from a function call
        /// </summary>
        /// <param name="content">The content from the function</param>
        /// <param name="function">The name of the function</param>
        public void AddFunctionMessage(string content, FunctionCall function)
        {
            Messages.Add(new Message(Role.Function, content, function.Name, null));
            Communicator.InvokeOnSystemMessageAdded(this, content);
        }

        /// <summary>
        /// Will add message coming from the assistant
        /// </summary>
        /// <param name="content">The text content of the message</param>
        public void AddAssistantMessage(string content)
        {
            Messages.Add(new Message(Role.Assistant, content));
        }

        /// <summary>
        /// Will add message coming from the user
        /// </summary>
        /// <param name="content">The text content of the message</param>
        public void AddUserMessage(string content)
        {
            Messages.Add(new Message(Role.User, content));
        }

        public async Task<string> EnterWorkflowAsync(string workflowName)
        {
            if (WorkflowProvider == null) throw new Exception("An attempt to enter workflow mode was made but no workflow provider was set");

            Workflow = await WorkflowProvider.GetWorkflowAsync(workflowName);

            if (Workflow == null) return $"Workflow {workflowName} was not found. No workflow was started.";

            Conversation workflowConversation = new Conversation(OpenAi, Model, TokenLimit);
            ChildConversation = workflowConversation;

            workflowConversation.Communicator = Communicator;
            workflowConversation.ParentConversation = this;
            workflowConversation.AddSystemMessage("Workflow mode was entered");
            workflowConversation.Workflow = Workflow;

            PreCompiledScriptProvider scriptProvider = new PreCompiledScriptProvider(typeof(GoToNextStepScript), typeof(ExitWorkflowScript));
            FunctionScriptLookup workflowScriptLookup = new FunctionScriptLookup(scriptProvider);
            workflowScriptLookup.CombineWith(FunctionLookup);

            try
            {
                await workflowScriptLookup.LoadFunctionsAsync();
            }
            catch (Exception exception)
            {
                return $"Workflow {workflowName} was not started. An error occured while loading the workflow: {exception.Message}";
            }

            workflowConversation.SetFunctionLookup(workflowScriptLookup);
            workflowConversation.AddSystemMessage(workflowConversation.Workflow.GoToNextStep(workflowConversation));

            return $"Workflow started: {workflowName}";
        }

        public string ExitWorkflow(string exitMessage)
        {
            if (ParentConversation != null)
                ParentConversation.AddSystemMessage($"Workflow exited: {exitMessage}");

            return $"Workflow exited: {exitMessage}";
        }

        /// <summary>
        /// Will complete on this conversation using the provided context for the scripts
        /// Completing is the term used by OpenAi for generating a new message or function call from a given conversation
        /// </summary>
        /// <param name="context">The context to give to the scripts that might be run</param>
        /// <returns>Nothing</returns>
        public async Task CompleteAsync(ScriptContext context)
        {
            try
            {
                Conversation conversation = GetActiveConversation();

                CompletionResult result = await OpenAi.CompleteAsync(conversation);

                foreach (Choice choice in result.Choices)
                {
                    if (choice.FinishReason == FinishReason.FunctionCall)
                    {
                        FunctionCall? functionCall = choice.Message.FunctionCall;

                        if (functionCall == null)
                            throw new Exception("Badly formatted answer from OpenAi. It said there would be a function call but the function was missing");
                        if (conversation.FunctionLookup == null)
                            throw new Exception("FunctionLookup is null even though a function is being called");

                        if (conversation.FunctionLookup.TryGetCompiledScriptContainer(functionCall.Name, out ICompiledScriptContainer scriptContainer))
                        {
                            Communicator.InvokeOnFunctionCallWasMade(this, functionCall);

                            CompiledScript compiledScript = scriptContainer.GetCompiledScript(context);

                            bool shouldComplete = true; // can be set by a ScriptResult object
                            try
                            {
                                object? returnValue = compiledScript.Run(functionCall.Arguments);

                                if (returnValue != null && returnValue.GetType() == typeof(ScriptResult)) // so that we don't always complete
                                {
                                    ScriptResult scriptResult = (ScriptResult)returnValue;
                                    shouldComplete = scriptResult.Complete;
                                    returnValue = scriptResult.Value;
                                }

                                conversation.AddFunctionMessage($"(function call returned: {ReturnValueConverter.GetStringFromObject(returnValue)})", functionCall);
                            }
                            catch (Exception exception)
                            {
                                conversation.AddFunctionMessage($"The function threw an exception and the user needs to be informed: {exception.Message} {exception.InnerException?.Message}", functionCall);
                            }

                            if (shouldComplete)
                                await CompleteAsync(context);
                        }
                    }
                    else
                    {
                        Communicator.InvokeOnCompletionMessageRecieved(this, choice.Message.Content);
                    }
                }
            }
            catch (Exception exception)
            {
                Communicator.InvokeOnErrorOccured(this, $"{exception.Message} {exception.InnerException?.Message}");
            }
        }

        private Conversation GetActiveConversation()
        {
            Conversation targetConversation = this;

            while (targetConversation.ChildConversation != null) // always take the inner most conversation
                targetConversation = targetConversation.ChildConversation;

            return targetConversation;
        }
    }
}
