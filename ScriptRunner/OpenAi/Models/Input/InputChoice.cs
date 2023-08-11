using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Input
{
    public class InputChoice
    {
        [JsonIgnore]
        public object? Value { get; set; }

        [JsonPropertyName("displayValue")]
        public string DisplayValue { get; set; }

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

        public override string ToString()
        {
            return DisplayValue;
        }
    }
}
