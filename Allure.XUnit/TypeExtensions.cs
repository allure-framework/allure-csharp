using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Allure.XUnit
{
    internal static class TypeExtensions
    {
        public static string GetFullFormattedTypeName(this Type type, Func<string, string> namingRule = null)
        {
            namingRule ??= typeName => typeName;
            if (!type.IsGenericType)
            {
                return namingRule.Invoke(type.Name);
            }

            var nameBuilder = new StringBuilder();
            BuildGenericTypeName(type, nameBuilder, namingRule);
            return nameBuilder.ToString();
        }

        private static void BuildGenericTypeName(Type type, StringBuilder nameBuilder, Func<string, string> namingRule)
        {
            if (!type.IsGenericType)
            {
                return;
            }

            StartGenericType(type, nameBuilder, namingRule);
            for (var index = 0; index < type.GenericTypeArguments.Length; index++)
            {
                var genericTypeArgument = type.GenericTypeArguments[index];
                if (genericTypeArgument.IsGenericType)
                {
                    BuildGenericTypeName(genericTypeArgument, nameBuilder, namingRule);
                    AppendDelimiterIfNeeded(type.GenericTypeArguments, nameBuilder, index);
                    continue;
                }

                nameBuilder.Append(namingRule.Invoke(genericTypeArgument.Name));
                AppendDelimiterIfNeeded(type.GenericTypeArguments, nameBuilder, index);
            }

            EndGenericType(nameBuilder);
        }

        private static void StartGenericType(
            MemberInfo type,
            StringBuilder nameBuilder,
            Func<string, string> namingRule)
        {
            var typeName = namingRule.Invoke(type.Name.Substring(0, type.Name.IndexOf("`", StringComparison.Ordinal)));
            nameBuilder.Append(typeName).Append('<');
        }

        private static void EndGenericType(StringBuilder nameBuilder)
        {
            nameBuilder.Append('>');
        }

        private static void AppendDelimiterIfNeeded(
            IReadOnlyCollection<Type> genericTypeArguments,
            StringBuilder nameBuilder,
            int index)
        {
            if (genericTypeArguments.Count - index != 1)
            {
                nameBuilder.Append(", ");
            }
        }
    }
}