using System;
using System.Reflection;
using System.Text;

namespace ScriptRunner.Helpers
{
    public class MethodInfoConverter
    {
        public static string GetMethodHeader(MethodInfo methodInfo)
        {
            string name = methodInfo.Name;

            StringBuilder parameterBuilder = new StringBuilder();

            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                parameterBuilder.Append($"{ConvertToBasicTypeName(parameter.ParameterType.ToString())} {parameter.Name}, ");
            }

            if (parameterBuilder.Length > 2)
                parameterBuilder.Length = parameterBuilder.Length - 2;

            return $"{name}({parameterBuilder})";
        }

        private static string ConvertToBasicTypeName(string typeName)
        {
            switch (typeName)
            {
                case "System.Boolean":
                    return "bool";
                case "System.Byte":
                    return "byte";
                case "System.SByte":
                    return "sbyte";
                case "System.Char":
                    return "char";
                case "System.Decimal":
                    return "decimal";
                case "System.Double":
                    return "double";
                case "System.Single":
                    return "float";
                case "System.Int32":
                    return "int";
                case "System.UInt32":
                    return "uint";
                case "System.Int64":
                    return "long";
                case "System.UInt64":
                    return "ulong";
                case "System.Int16":
                    return "short";
                case "System.UInt16":
                    return "ushort";
                case "System.String":
                    return "string";
                default:
                    return typeName;
            }
        }
    }
}
