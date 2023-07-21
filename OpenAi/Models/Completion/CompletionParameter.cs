using System.Text.Json.Serialization;

namespace OpenAi.Models.Completion
{
    /// <summary>
    /// A parameter to use for sending a completion request to the OpenAI API.
    /// </summary>
    public class CompletionParameter
    {
        /// <summary>
        /// The model to use for the completion
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// The messages that has been in the conversation so far
        /// </summary>
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        /// <summary>
        /// The functions that can be called
        /// </summary>
        [JsonPropertyName("functions")]
        public List<Function>? Functions { get; set; }

        /// <summary>
        /// Determines how OpenAI will handle functions. "auto" means it will automatically determine. Use the SetFunctionCall method to specify a function that parameters should be collected for
        /// </summary>
        [JsonPropertyName("function_call")]
        public string FunctionCall { get; set; }

        /// <summary>
        /// Constructor for a completion parameter
        /// </summary>
        /// <param name="model">The model to use</param>
        /// <param name="messages">The messages that has been in the conversation</param>
        /// <param name="functions">The functions that can be called</param>
        public CompletionParameter(string model, List<Message> messages, List<Function>? functions)
        {
            Model = model;
            Messages = messages;
            Functions = functions;
            FunctionCall = "auto";
        }

        /// <summary>
        /// Constructor for a completion parameter
        /// </summary>
        /// <param name="model">The name of the model to use</param>
        public CompletionParameter(string model)
        {
            Model = model;
            Messages = new List<Message>();
            FunctionCall = "auto";
        }

        /// <summary>
        /// Will add a message to the conversation
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        /// <summary>
        /// Will add message coming from the system
        /// </summary>
        /// <param name="content">The text content of the message</param>
        public void AddSystemMessage(string content)
        {
            Messages.Add(new Message(Role.System, content));
        }

        /// <summary>
        /// Will add message coming from the assistant
        /// </summary>
        /// <param name="content">The text content of the message</param>
        public void AddAssistantMessage(string content)
        {
            Messages.Add(new Message(Role.Assistant, content));
        }

        /// <summary>
        /// Will add message coming from the user
        /// </summary>
        /// <param name="content">The text content of the message</param>
        public void AddUserMessage(string content)
        {
            Messages.Add(new Message(Role.User, content));
        }

        /// <summary>
        /// Will add a function that can be called
        /// </summary>
        /// <param name="function">The function that can be called</param>
        public void AddFunction(Function function)
        {
            if(Functions == null) 
                Functions = new List<Function>();

            Functions.Add(function);
        }

        /// <summary>
        /// Will set the FunctionCall property to the name of a specific function, this will cause the API to collect parameters for that function
        /// </summary>
        /// <param name="functionName">The name of the function to collect parameters for</param>
        public void SetFunctionCall(string functionName)
        {
            FunctionCall = string.Format("{\"name\": \"{0}\"", functionName);
        }
    }
}
