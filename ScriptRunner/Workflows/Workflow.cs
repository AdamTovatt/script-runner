using System.Text;

namespace ScriptRunner.Workflows
{
    public class Workflow
    {
        public List<string> Tasks { get; set; }
        public string Name { get; set; }
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

        public string GoToNextStep()
        {
            currentStep++;

            if (currentStep > Tasks.Count)
                return "Workflow completed, call the function to exit the work flow with a message about how the workflow went";

            StringBuilder message = new StringBuilder();

            message.AppendLine($"You are currently in a workflow that will {Purpose}");
            message.AppendLine("A workflow is a collection of tasks that you will complete, if the task requires you to take input from the use you will do so. ");
            message.AppendLine("When you have aquired all neccessary inputs from the user and completed everything there is to do in the current task you will continue by calling the GoToNextStep function.");
            message.AppendLine($"If the users wants to exit the workflow prematurely they can. Maybe they change their mind and don't want to continue. You can help them by calling first warning them that if they exit the progress towards {Purpose} will be lost, if they still want to exit you can call the ExitWorkflow() function with a short message about what happened.");

            if (savedValues.Count > 0)
            {
                message.AppendLine("As a reminder, here are the values you have collected from the user so far: ");

                foreach (KeyValuePair<string, object> keyValuePair in savedValues)
                    message.AppendLine($"{keyValuePair.Key}: {keyValuePair.Value}");
            }

            message.AppendLine();
            message.AppendLine("Now, here is your current task: ");
            message.AppendLine(Tasks[currentStep - 1].ToString());

            return message.ToString();
        }

        public void SaveValue(string key, object value)
        {
            if (savedValues.ContainsKey(key))
                savedValues[key] = value;
            else
                savedValues.Add(key, value);
        }
    }
}