namespace OpenAi.Models.Completion
{
    public class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; }
        public FinishReason FinishReason { get; set; }

        public Choice(int index, Message message, FinishReason finishReason)
        {
            Index = index;
            Message = message;
            FinishReason = finishReason;
        }
    }
}
