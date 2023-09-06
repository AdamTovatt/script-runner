using ScriptRunner;
using ScriptRunner.DocumentationAttributes;

namespace SkippyBackend.PrecompiledScripts
{
    public class SubmitReceiptScript : CompiledScript
    {
        public SubmitReceiptScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Parameter("totalPrice", "The total price found in the receipt image text. ")]
        [Parameter("dateOfPurchase", "The date of purchase found in the receipt image text. ")]
        public string SubmitReceipt(string totalPrice, string dateOfPurchase)
        {
            return $"TotalPrice: {totalPrice} \nDateOfPurchase: {dateOfPurchase}";
        }
    }
}
