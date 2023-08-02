namespace ScriptRunner.OpenAi.Models.Completion
{
    public class Communicator
    {
        public delegate void CompletionMessageRecievedHandler(object sender, string message);
        public event CompletionMessageRecievedHandler? OnCompletionMessageRecieved;

        public delegate void FunctionCallWasMadeHandler(object sender, FunctionCall functionCall);
        public event FunctionCallWasMadeHandler? OnFunctionCallWasMade;

        public delegate void ErrorOccuredHandler(object sender, string message);
        public event ErrorOccuredHandler? OnErrorOccured;

        public delegate void SystemMessageAddedHandler(object sender, string message);
        public event SystemMessageAddedHandler? OnSystemMessageAdded;

        public delegate void WantsInputHandler(object sender, Type inputType, string inputMessage);
        public event WantsInputHandler? OnWantsInput;

        public void InvokeOnCompletionMessageRecieved(object sender, string message)
        {
            OnCompletionMessageRecieved?.Invoke(sender, message);
        }

        public void InvokeOnFunctionCallWasMade(object sender, FunctionCall functionCall)
        {
            OnFunctionCallWasMade?.Invoke(sender, functionCall);
        }

        public void InvokeOnErrorOccured(object sender, string message)
        {
            OnErrorOccured?.Invoke(sender, message);
        }

        public void InvokeOnSystemMessageAdded(object sender, string message)
        {
            OnSystemMessageAdded?.Invoke(sender, message);
        }

        /// <summary>
        /// Will return true if the event was invoked, false if not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="type"></param>
        /// <param name="inputMessage"></param>
        /// <returns></returns>
        public bool InvokeOnWantsInput(object sender, Type type, string inputMessage)
        {
            if (OnWantsInput == null) return false;

            OnWantsInput.Invoke(sender, type, inputMessage);
            return true;
        }
    }
}
