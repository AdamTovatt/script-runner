﻿using ScriptRunner.Models;
using ScriptRunner.OpenAi.Models.Input;

namespace ScriptRunner.OpenAi.Models.Completion
{
    public class Communicator
    {
        public delegate void CompletionMessageRecievedHandler(object sender, string message);
        /// <summary>
        /// Will be called when a completion message was received (this is a message from the AI)
        /// </summary>
        public event CompletionMessageRecievedHandler? OnCompletionMessageRecieved;

        public delegate void FunctionCallWasMadeHandler(object sender, FunctionCall functionCall);
        /// <summary>
        /// Will be called when a function call was made
        /// </summary>
        public event FunctionCallWasMadeHandler? OnFunctionCallWasMade;

        public delegate void ErrorOccuredHandler(object sender, string message);
        /// <summary>
        /// Will be called when an error occured
        /// </summary>
        public event ErrorOccuredHandler? OnErrorOccured;

        public delegate void SystemMessageAddedHandler(object sender, string message);
        /// <summary>
        /// Will be called when a system message was added to the chat
        /// </summary>
        public event SystemMessageAddedHandler? OnSystemMessageAdded;

        public delegate void WantsInputHandler(InputHandler sender, InputInfo inputInfo);
        /// <summary>
        /// Will be called when a script wants input
        /// </summary>
        public event WantsInputHandler? OnWantsInput;

        public delegate void InputSubmittedHandler(object sender, InputResult inputResult);
        /// <summary>
        /// Will be called when input was submitted
        /// </summary>
        public event InputSubmittedHandler? OnInputSubmitted;

        public delegate void FileWasSentHandler(object sender, byte[] content, ContentType fileType, string fileName);
        /// <summary>
        /// Will be called when a file was sent
        /// </summary>
        public event FileWasSentHandler? OnFileWasSent;

        public delegate void ResponseSuggestionsWereSent(object sender, string[] suggestions);
        /// <summary>
        /// Will be called when response suggestions are sent to the chat
        /// </summary>
        public event ResponseSuggestionsWereSent? OnResponseSuggestionsWereSent;

        /// <summary>
        /// This method can be called to send a list of input suggestions to the chat
        /// </summary>
        /// <param name="sender">The origin of the event</param>
        /// <param name="suggestions">The suggestions to send to the chat</param>
        public void InvokeOnResponseSuggestionsWereSent(object sender, string[] suggestions)
        {
            OnResponseSuggestionsWereSent?.Invoke(sender, suggestions);
        }

        /// <summary>
        /// This method can be called to send a message from the bot to the chat
        /// </summary>
        /// <param name="sender">The origin of the event</param>
        /// <param name="message">The message that is to be sent to the chat</param>
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

        public void InvokeOnFileWasSent(object sender, byte[] content, ContentType fileType, string fileName)
        {
            OnFileWasSent?.Invoke(sender, content, fileType, fileName);
        }

        /// <summary>
        /// Will return true if the event was invoked, false if not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="type"></param>
        /// <param name="inputMessage"></param>
        /// <returns></returns>
        public bool InvokeOnWantsInput(InputHandler sender, InputInfo inputInfo)
        {
            if (OnWantsInput == null) return false;

            OnWantsInput.Invoke(sender, inputInfo);
            return true;
        }

        public void InvokeInputSubmitted(object sender, InputResult inputResult)
        {
            OnInputSubmitted?.Invoke(sender, inputResult);
        }
    }
}
