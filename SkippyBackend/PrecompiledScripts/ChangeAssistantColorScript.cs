using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using SkippyBackend.Models;
using System.Text.RegularExpressions;

namespace SkippyBackend.PrecompiledScripts
{
    public class ChangeAssistantColorScript : CompiledScript
    {
        public ChangeAssistantColorScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will change the color of the messages for the assistant")]
        [Parameter("color", "The color to change the response to. Must be a hexadecimal string starting with #")]
        public string ChangeAssistantMessageColor(string color)
        {
            SkippyContext context = (SkippyContext)Context;

            if (!Regex.IsMatch(color, @"^#(?:[0-9a-fA-F]{3}){1,2}$"))
                return $"Invalid color provided: {color} doesn't match the correct format";

            context.ClientData.ChatConfiguration.Colors["Accent1"] = color;
            return "Changed the color of the responses";
        }
    }
}
