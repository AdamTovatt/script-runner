using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Input
{
    public class InputChoice
    {
        [JsonIgnore]
        public object? Value { get; set; }

        [JsonPropertyName("displayValue")]
        public string DisplayValue { get; set; }

        [JsonPropertyName("metadata")]
        public string? Metadata { get; set; }

        public InputChoice(object? value, string displayValue, object metadata)
        {
            Value = value;
            DisplayValue = displayValue;

            if (metadata != null)
                Metadata = JsonSerializer.Serialize(metadata);
        }

        public InputChoice(object? value, string displayValue, string metadata)
        {
            Value = value;
            DisplayValue = displayValue;
            Metadata = metadata;
        }

        public InputChoice(object? value, string displayValue, IMetadata metadata)
        {
            Value = value;
            DisplayValue = displayValue;
            Metadata = metadata.Serialize();
        }

        public InputChoice(object? value, string displayValue)
        {
            Value = value;
            DisplayValue = displayValue;
        }

        public InputChoice(object? value)
        {
            Value = value;
            DisplayValue = value?.ToString() ?? "(null)";
        }

        /// <summary>
        /// Will create a list of input choices given a list of some other object and a function to create the input choice from that object.
        /// </summary>
        /// <typeparam name="T">The type of objects to create input choices from</typeparam>
        /// <param name="products">A list of objects of the type T to create input choices from</param>
        /// <param name="choiceCreator">The function that creates an input choices from a object of type T</param>
        /// <returns>A list of input choices</returns>
        public static List<InputChoice> CreateList<T>(List<T> products, Func<T, InputChoice> choiceCreator)
        {
            List<InputChoice> inputChoices = new List<InputChoice>();

            foreach (T item in products)
            {
                inputChoices.Add(choiceCreator(item));
            }

            return inputChoices;
        }

        public override string ToString()
        {
            return DisplayValue;
        }
    }
}
