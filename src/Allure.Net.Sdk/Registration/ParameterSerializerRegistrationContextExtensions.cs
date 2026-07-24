using System;
using System.Collections.Generic;
using System.Text.Json;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Registration;

public static class ParameterSerializerRegistrationContextExtensions
{
    extension (IParameterSerializerRegistrationContext registration)
    {
        public void AddRules(
            params IEnumerable<IParameterSerializationRule> rules
        )
        {
            foreach (var rule in rules)
            {
                registration.AddRule(rule);
            }
        }

        public void AddLambdaRule<TValue>(
            Func<TValue, string> serialize
        ) =>
            registration.AddRule(LambdaParameterSerializationRule.Create(serialize));

        public void AddJsonRule(JsonSerializerOptions jsonOptions) =>
            registration.AddRule(new JsonParameterSerializationRule(jsonOptions));

        public void AddToStringRule() =>
            registration.AddRule(ToStringParameterSerializationRule.Instance);
    }
}
