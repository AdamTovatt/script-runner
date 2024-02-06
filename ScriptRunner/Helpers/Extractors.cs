using System.Text;

namespace ScriptRunner.Helpers
{
    public class Extractors
    {
        /// <summary>
        /// Will extract ints from strings by removing all non digit characters
        /// </summary>
        public static Func<string, object?> SimpleIntExtractor => (input) =>
        {
            if (string.IsNullOrEmpty(input))
                return null;

            StringBuilder cleanedInput = new StringBuilder();

            foreach (char character in input)
            {
                if (char.IsDigit(character))
                    cleanedInput.Append(character);
            }

            if (int.TryParse(cleanedInput.ToString(), out int result))
                return result;

            return null;
        };
    }
}
