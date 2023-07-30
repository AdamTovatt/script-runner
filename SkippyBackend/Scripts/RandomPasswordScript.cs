using ScriptRunner;
using System;

namespace CustomScripts
{
    public class RandomPasswordScript : CompiledScript
    {
        public RandomPasswordScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script generates a random password.
        /// </summary>
        /// <param name="length">The length of the password to generate</param>
        [ScriptStart]
        public object GenerateRandomPassword(int length)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            char[] password = new char[length];
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                password[i] = allowedChars[random.Next(allowedChars.Length)];
            }

            return new string(password);
        }
    }
}
