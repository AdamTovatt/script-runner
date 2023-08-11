using System.Collections;

namespace ScriptRunner.OpenAi.Helpers
{
    public static class ExtensionMethods
    {
        public static string GetJsonTypeName(this Type type)
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

        public static bool IsAnyOf(this Type type, params Type[] types)
        {
            foreach (Type singleType in types)
                if (type == singleType) return true;

            return false;
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsClass || type.IsInterface || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
