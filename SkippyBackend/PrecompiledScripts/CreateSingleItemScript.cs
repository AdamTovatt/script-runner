using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.OpenAi;
using ScriptRunner.OpenAi.Models.Input;

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

            if (Context == null || Context.Conversation == null) // this should never happen
                return "Something went wrong, please try again later.";
            
            string? item = (await Context.Conversation.Input.GetAsync<string?>("What item do you want to insure?", true, retryPromptMessage: "Sorry, I didn't understand how that's an item that can be insured, can you please clarify the item you want to insure?"));

            decimal itemValue = (await Context.Conversation.Input.GetAsync<decimal?>($"What would your {item} cost to replace?", true, retryPromptMessage: "That's not valid number that I can understand as the value of your item, please clarify. The value of the item. ")).Value;

            bool hadRecentClaims = (await Context.Conversation.Input.GetAsync<bool?>("Have you had any recent claims?", true, retryPromptMessage: "Is that a yes or a no?")).Value;
            int recentClaims = 0;

            if (hadRecentClaims)
            {
                List<InputChoice> recentClaimsChoices = new List<InputChoice>()
                {
                    new InputChoice(1, "Only one"),
                    new InputChoice(2, "Two"),
                    new InputChoice(3, "Three or more"),
                };
                recentClaims = (await Context.Conversation.Input.GetAsync<int?>("Okay, how many claims have you had in the past 12 months then?", true, choices: recentClaimsChoices, retryPromptMessage: "I'm sorry, that's not number I recognize, please clarify how many claims you have had in the past 12 months. ")).Value;
            }

            return $"A single item cover was created for the item \"{item}\" with the value of {itemValue}£ with {recentClaims} recent claims. (not really)";
        }
    }
}
