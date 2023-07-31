using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Completion
{
    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }

        public Choice(int index, Message message, string finishReason)
        {
            Index = index;
            Message = message;
            FinishReason = finishReason;
        }

        public override string ToString()
        {
            return Message.ToString();
        }
    }
}
