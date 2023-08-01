using ScriptRunner.Helpers;
using ScriptRunner.Models;
using ScriptRunner.OpenAi.Helpers;
using ScriptRunner.Providers;
using ScriptRunner.ScriptConvertion;
using ScriptRunner.Workflows;
using ScriptRunner.Workflows.Scripts;
using System.Text.Json;

namespace ScriptRunner.OpenAi.Models.Completion
{
    public class Conversation
    {
        public delegate void CompletionMessageRecievedHandler(object sender, string message);
        public event CompletionMessageRecievedHandler? OnCompletionMessageRecieved;

        public delegate void FunctionCallWasMadeHandler(object sender, FunctionCall functionCall);
        public event FunctionCallWasMadeHandler? OnFunctionCallWasMade;

        public delegate void ErrorOccuredHandler(object sender, string message);
        public event ErrorOccuredHandler? OnErrorOccured;

        public OpenAiApi OpenAi { get; set; }
        public List<Message> Messages { get; set; }
        public List<Function>? Functions { get { if (FunctionLookup == null) return null; return FunctionLookup.GetFunctions(); } }
        public FunctionScriptLookup? FunctionLookup { get; set; }
        public string Model { get; set; }
        public int? TokenLimit { get; set; }
        public Conversation? ParentConversation { get; set; }
        public Conversation? ChildConversation { get; set; }
        public Workflow? Workflow { get; set; }
        public IWorkflowProvider? WorkflowProvider { get; set; }
        public Conversation ActiveConversation { get { return GetActiveConversation(); } }

        private TokenCounter tokenCounter;

        public Conversation(OpenAiApi openAi, string model, int? tokenLimit = null)
        {
            OpenAi = openAi;
            TokenLimit = tokenLimit;
            Model = model;
            Messages = new List<Message>();
            tokenCounter = new TokenCounter(model);
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

                    Messages.Add(new Message(Role.System, functionArguments == null ? "(Calling function)" : $"(Calling function, paramters: {functionArguments})"));
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

            workflowConversation.ParentConversation = this;
            workflowConversation.AddSystemMessage("Workflow mode was entered");
            workflowConversation.Workflow = Workflow;

            PreCompiledScriptProvider scriptProvider = new PreCompiledScriptProvider(typeof(GoToNextStepScript), typeof(ExitWorkflowScript));
            FunctionScriptLookup workflowScriptLookup = new FunctionScriptLookup(scriptProvider);

            try
            {
                await workflowScriptLookup.LoadFunctionsAsync();
            }
            catch(Exception exception)
            {

            }

            workflowScriptLookup.CombineWith(FunctionLookup);
            workflowConversation.SetFunctionLookup(workflowScriptLookup);
            workflowConversation.AddSystemMessage(workflowConversation.Workflow.GoToNextStep());

            return $"Workflow started: {workflowName}";
        }

        public string ExitWorkflow(string exitMessage)
        {
            if (ParentConversation != null)
                ParentConversation.AddSystemMessage($"Workflow exited: {exitMessage}");

            return $"Workflow exited: {exitMessage}";
        }

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
                            OnFunctionCallWasMade?.Invoke(this, functionCall);

                            CompiledScript compiledScript = scriptContainer.GetCompiledScript(context);

                            try
                            {
                                object? returnValue = compiledScript.Run(functionCall.Arguments);
                                conversation.AddSystemMessage($"Function call returned: {ReturnValueConverter.GetStringFromObject(returnValue)}");
                            }
                            catch (Exception exception)
                            {
                                conversation.AddSystemMessage($"The function threw an exception and the user needs to be informed: {exception.Message} {exception.InnerException?.Message}");
                            }

                            await CompleteAsync(context);
                        }
                    }
                    else
                    {
                        OnCompletionMessageRecieved?.Invoke(this, choice.Message.Content);
                    }
                }
            }
            catch (Exception exception)
            {
                OnErrorOccured?.Invoke(this, $"{exception.Message} {exception.InnerException?.Message}");
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
