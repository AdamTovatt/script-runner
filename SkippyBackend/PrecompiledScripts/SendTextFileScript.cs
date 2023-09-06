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
        public void SendTextFile(string textContent, string fileName)
        {
            Context.Conversation.Communicator.InvokeOnFileWasSent(this, Encoding.UTF8.GetBytes(textContent), ContentType.File, fileName);
        }
    }
}
