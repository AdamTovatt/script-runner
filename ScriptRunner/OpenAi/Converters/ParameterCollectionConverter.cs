﻿using ScriptRunner.OpenAi.Models.Completion.Parameters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Converters
{
    /// <summary>
    /// Used to convert a ParameterCollection to JSON
    /// </summary>
    public class ParameterCollectionConverter : JsonConverter<ParameterCollection>
    {
        public override ParameterCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ParameterCollection value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(value.ToJson());
        }
    }
}
