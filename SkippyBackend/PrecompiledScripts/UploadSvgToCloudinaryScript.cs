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
        [Parameter("svgContent", "The content of the svg file. ")]
        public async Task<string> UploadSvgToCloudinary(string svgName, string svgContent)
        {
            return await CloudinaryHelper.Instance.UploadImageAsync(svgName, UTF8Encoding.UTF8.GetBytes(svgContent));
        }
    }
}
