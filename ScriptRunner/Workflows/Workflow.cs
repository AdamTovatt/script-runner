using ScriptRunner.OpenAi.Models.Completion;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptRunner.Workflows
{
    public class Workflow
    {
        [JsonPropertyName("tasks")]
        public List<string> Tasks { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }

        private int currentStep = 1;
        private Dictionary<string, object> savedValues;

        public Workflow(string name, List<string> tasks, string purpose)
        {
            Name = name;
            Tasks = tasks;
            Purpose = purpose;

            savedValues = new Dictionary<string, object>();
        }

        public string GetShortCurrentTaskPrompt()
        {
            if (currentStep > Tasks.Count)
                return "Workflow completed, call the function to exit the work flow with a message about how the workflow went";

            return $"Your current task is: {Tasks[currentStep - 1]}\n Call GoToNextStep() if you have done that";
        }

        public string GetPromptForCurrentTask(Conversation conversation)
        {
            if (currentStep > Tasks.Count)
                return "Workflow completed, call the function to exit the work flow with a message about how the workflow went";

            conversation.AddUserMessage(Tasks[currentStep - 1].ToString());

            return "";
        }

        public string GoToNextStep(Conversation conversation)
        {
            currentStep++;

            return GetPromptForCurrentTask(conversation);
        }

        public void SaveValue(string key, object value)
        {
            if (savedValues.ContainsKey(key))
                savedValues[key] = value;
            else
                savedValues.Add(key, value);
        }

        public static Workflow? FromJson(string json)
        {
            return JsonSerializer.Deserialize<Workflow>(json);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}