using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.OpenAi;
using ScriptRunner.OpenAi.Models.InputTypes;

namespace SkippyBackend.PrecompiledScripts
{
    public class CreateSingleItemScript : CompiledScript
    {
        public CreateSingleItemScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will create a single item cover for the user, this is a type of insurance. If the user wants to insure something, call this function. ")]
        public async Task<string> CreateSingleItem()
        {
            OpenAiApi ai = Context.Conversation.OpenAi; // for shorter code later :weary:

            string rawItem = await Context.Conversation.GetInputFromUser<string>("What item do you want to insure?");
            string item = (await ai.ExtractAsync<StringInputType>(rawItem)).ExtractedValue!.Value;

            string rawItemValue = await Context.Conversation.GetInputFromUser<string>($"What would your {item} cost to replace?");
            decimal itemValue = (await ai.ExtractAsync<DecimalInputType>(rawItemValue)).ExtractedValue!.Value;

            string rawRecentClaims = await Context.Conversation.GetInputFromUser<string>("Have you had any recent claims?");
            bool recentClaims = (await ai.ExtractAsync<BoolInputType>(rawRecentClaims)).ExtractedValue!.Value;

            return $"A single item cover was created for the item \"{item}\" with the value of {itemValue}£ and recent claims is: {recentClaims}. (not really)";
        }
    }
}
