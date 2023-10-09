using System.Collections;
using System.Text;
using System.Text.Json;

namespace ScriptRunner.Helpers
{
    /// <summary>
    /// Used to convert a return value
    /// </summary>
    public static class ReturnValueConverter
    {
        /// <summary>
        /// Converts a return value of the type object? to a string
        /// </summary>
        /// <param name="value">The return value to convert</param>
        /// <returns>A string representing the value</returns>
        public static string GetStringFromObject(object? value)
        {
            if (value == null)
                return "null";

            if (value is string stringValue)
                return stringValue;

            if (typeof(IEnumerable).IsAssignableFrom(value.GetType()))
            {
                StringBuilder sb = new StringBuilder();

                foreach (object? item in (IEnumerable)value)
                    sb.AppendLine($"{GetStringFromObject(item)},");

                if (sb.Length == 0) return "(empty list)";

                sb.Length = sb.Length - 1;

                return sb.ToString();
            }

            if (value.GetType().GetMethod("ToString")?.DeclaringType == value.GetType()) // check if the type has overriden ToString()
                return value.ToString() ?? "null"; // if a ToString() method has been explicitly declared, use that

            return JsonSerializer.Serialize(value); // otherwise, serialize to json
        }
    }
}
