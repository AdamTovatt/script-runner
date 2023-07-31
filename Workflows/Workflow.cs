namespace Workflows
{
    public class Workflow
    {
        public List<string> Tasks { get; set; }
        public string Name { get; set; }

        public Workflow(string name, List<string> tasks)
        {
            Name = name;
            Tasks = tasks;
        }
    }
}