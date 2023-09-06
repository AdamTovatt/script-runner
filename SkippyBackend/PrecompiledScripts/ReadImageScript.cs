using IronOcr;
using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.Models;
using ScriptRunner.OpenAi.Models.Completion;

namespace SkippyBackend.PrecompiledScripts
{
    public class ReadImageScript : CompiledScript
    {
        public ReadImageScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will use OCR to read and return the text in an image. Will automatically skip the reading of the image and just return the result text which needs to be analyzed. ")]
        public async Task<string> ReadImage()
        {
            Context.Conversation.AddAssistantMessage("Give me the image");
            byte[]? image = await Context.Conversation.Input.GetAsync<byte[]>("Give me the image", "image");
            Context.Conversation.AddUserMessage("[image content]");

            if (image == null)
            {
                Context.Conversation.Communicator.InvokeOnCompletionMessageRecieved(this, "No image was sent");
                return "Error, missing image";
            }

            Context.Conversation.Communicator.InvokeOnFileWasSent(this, image, ContentType.Image, "userImage.png");

            IronTesseract ocr = new IronTesseract();

            using (var ocrInput = new OcrInput())
            {
                ocrInput.AddImage(image);

                // Optionally Apply Filters if needed:
                ocrInput.Deskew();  // use only if image not straight
                ocrInput.DeNoise(); // use only if image contains digital noise

                var ocrResult = ocr.Read(ocrInput);

                Conversation conversation = new Conversation(Context.Conversation.OpenAi, Model.Default);
                conversation.AddSystemMessage("After this text, there will be a receipt that has been scanned using OCR. Please respond with a JSON object containing the total price, date of purchase, your estimation of the items purchased, and the currency. Additionally, we would like to know the amount of the purchase that consists of VAT. For example:\r\n\r\n```\r\n{\r\n  \"total\": 100,\r\n  \"vat\": 25,\r\n  \"date\": \"2023-04-20\",\r\n  \"content\": \"Electronics\",\r\n  \"currency\": \"SEK\"\r\n}\r\n```\r\n\r\nPlease note that the date is usually in the format YY-MM-DD. Try to keep the \"content\" field short and include only one category. Now, here is the text: " + ocrResult.Text);

                await conversation.CompleteAsync(new ScriptContext(conversation));
                return conversation.Messages[1].Content;
            }
        }
    }
}
