using System;
using System.Collections.Immutable;
using Allure.Sdk.Registration;
using Allure.Xunit.Registration;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.RegistrationHooks.ReflectionHook;

public sealed class AllureRegistrationHook : IAllureXunitRegistrationHook
{
    public void SetUp(IAllureXunitRegistrationContext context)
    {
        context.TransformConfiguration(configuration => configuration.WithProperty(
            static configuration => configuration.GlobalLabels,
            ImmutableDictionary<string, string>.Empty.Add("hook", "true"),
            static (configuration, value) => configuration with
            {
                GlobalLabels = value,
            }
        ));
    }
}

public class TestClass
{
    [Fact]
    public void TestMethod() { }
}
