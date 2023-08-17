using ScriptRunner.OpenAi.Models.Completion;

namespace ScriptRunner.OpenAi.Helpers
{
    public static class ExtensionMethods
    {
        public static string GetJsonTypeName(this Type type)
        {
            return JsonType.GetStringValue(type);
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
