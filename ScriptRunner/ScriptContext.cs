using ScriptRunner.OpenAi.Models.Completion;

namespace ScriptRunner
{
    public class ScriptContext
    {
        public Conversation Conversation { get; set; }

        public ScriptContext(Conversation conversation)
        {
            Conversation = conversation;
        }
    }
}
