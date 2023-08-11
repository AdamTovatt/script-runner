using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using SkippyBackend.Helpers;
using System.Text;

namespace SkippyBackend.PrecompiledScripts
{
    public class UploadSvgToCloudinaryScript : CompiledScript
    {
        public UploadSvgToCloudinaryScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Parameter("svgName", "The name of the svg file. The name of the image." )]
        [AllowedRoles("admin")]
        public async Task<string> UploadSvgToCloudinary(string svgName)
        {
            try
            {
                string input = await Context.Conversation.Input.GetAsync<string>("Please provide the content of the svg file as a string in the chat");

                return await CloudinaryHelper.Instance.UploadImageAsync(svgName, Encoding.UTF8.GetBytes(input));
            }
            catch(Exception exception)
            {
                return $"Error when uploading svg: {exception.Message}";
            }
        }
    }
}
