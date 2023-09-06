using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.Models;
using System.Text;

namespace SkippyBackend.PrecompiledScripts
{
    public class SendTextFileScript : CompiledScript
    {
        public SendTextFileScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Parameter("textContent", "The text content of the file to send. ")]
        [Parameter("fileName", "The name of the file to send. Can be made up to be descriptive of the content. ")]
        public string SendTextFile(string textContent, string fileName)
        {
            Context.Conversation.AddAssistantMessage("[text file]");
            Context.Conversation.Communicator.InvokeOnFileWasSent(this, Encoding.UTF8.GetBytes(textContent), ContentType.File, fileName);
            return "A text file was sent in the chat";
        }
    }
}
