namespace OpenAi.Models.Completion
{
    public class CompletionException : Exception
    {
        HttpResponseMessage? HttpResponseMessage { get; set; }

        public CompletionException(string message, HttpResponseMessage? httpResponseMessage) : base(message)
        {
            HttpResponseMessage = httpResponseMessage;
        }
    }
}
