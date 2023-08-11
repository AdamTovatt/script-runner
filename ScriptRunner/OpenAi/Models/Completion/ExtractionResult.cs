using ScriptRunner.OpenAi.Models.Input.Types;
using System.Globalization;

namespace ScriptRunner.OpenAi.Models.Completion
{
    public class ExtractionResult<T> where T : IInputType
    {
        public T? ExtractedValue { get; set; }
        public string RawText { get; set; }
        public bool Valid { get; set; }

        public ExtractionResult(string message)
        {
            Valid = true;
            RawText = message;

            if (string.IsNullOrEmpty(message) || message.ToLower().Contains("invalid"))
            {
                Valid = false;
                return;
            }

            Type type = typeof(T);

            if (type == typeof(BoolInputType))
            {
                if (bool.TryParse(message.ToLower(), out bool boolResult))
                    ExtractedValue = (T)(IInputType)new BoolInputType(boolResult);
                else
                    Valid = false;
            }
            else if (type == typeof(DecimalInputType))
            {
                if (decimal.TryParse(message, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalResult))
                    ExtractedValue = (T)(IInputType)new DecimalInputType(decimalResult);
                else
                    Valid = false;
            }
            else if (type == typeof(IntegerInputType))
            {
                if (int.TryParse(message, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intResult))
                    ExtractedValue = (T)(IInputType)new IntegerInputType(intResult);
                else
                    Valid = false;
            }
            else if (type == typeof(StringInputType))
            {
                ExtractedValue = (T)(IInputType)new StringInputType(message.ToLower());
            }
            else
                throw new InvalidDataException($"Invalid type for extraction result: {type}");
        }
    }
}
