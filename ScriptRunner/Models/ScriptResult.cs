namespace ScriptRunner.Models
{
    /// <summary>
    /// Can be used to return a value from a script while also chosing if the conversation should be completed after the result is returned
    /// </summary>
    public class ScriptResult
    {
        /// <summary>
        /// Wether or not the conversation should be completed, should probably be false since there is no reason to use this class otherwise
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// The actual return value of the script
        /// </summary>
        public object? Value { get; set; }

        public ScriptResult(bool complete, object? value)
        {
            Complete = complete;
            Value = value;
        }
    }
}
