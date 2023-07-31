using OpenAi.Models.Completion;
using ScriptRunner;
using ScriptRunner.OpenAi.Models.Completion;
using ScriptRunner.ScriptConvertion;

namespace ScriptRunner.Workflows
{
    public class WorkflowContext : ScriptContext
    {
        public FunctionScriptLookup FunctionLookup { get; set; }
        public Conversation Conversation { get; set; }
        public Workflow? Workflow { get; set; }

        public WorkflowContext(Conversation conversation) : base()
        {
            Conversation = conversation;
        }

        public string EnterWorkflow(string workflowName)
        {
            Conversation.ChildConversation = new Conversation(Conversation.Model, Conversation.TokenLimit);
            Conversation.ChildConversation.AddSystemMessage("Workflow mode was entered");

            return $"Workflow started: {workflowName}";
        }

        public string ExitWorkflow(string exitMessage)
        {
            Conversation.AddSystemMessage($"Workflow exited: {exitMessage}");

            return $"Workflow exited: {exitMessage}";
        }
    }
}
