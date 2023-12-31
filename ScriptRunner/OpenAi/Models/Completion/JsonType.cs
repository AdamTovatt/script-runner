﻿using System.Collections;
using System.Text;

namespace ScriptRunner.OpenAi.Models.Completion
{
    public static class JsonType
    {
        private static IReadOnlyList<string> types = new List<string>() { "string", "number", "boolean", "array", "object", "any" };

        public static IReadOnlyList<string> GetPossibleValues()
        {
            return types;
        }

        public static bool IsValid(string type)
        {
            return types.Contains(type);
        }

        public static string GetPossibleValuesAsString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (string type in types)
                builder.Append($"{type}, ");

            builder.Length = builder.Length - 1;

            return builder.ToString();
        }

        public static string GetStringValue(Type type)
        {
            if (type == typeof(string))
            {
                return "string";
            }
            else if (type == typeof(int) || type == typeof(long) || type == typeof(decimal) || type == typeof(float) || type == typeof(double))
            {
                return "number";
            }
            else if (type == typeof(bool))
            {
                return "boolean";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
            {
                return "string";
            }
            else if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) || typeof(IEnumerable).IsAssignableFrom(type))
            {
                return "array";
            }
            else if (type.IsClass)
            {
                return "object";
            }
            else
            {
                return "any";
            }
        }
    }
}
