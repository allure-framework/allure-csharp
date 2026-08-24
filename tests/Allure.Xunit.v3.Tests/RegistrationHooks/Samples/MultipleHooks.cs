using System;
using System.Runtime.CompilerServices;
using Allure.Sdk.Registration;
using Allure.Xunit.Registration;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.RegistrationHooks.MultipleHooks;

public class AllureRegistrationHook(string value) : IAllureXunitRegistrationHook
{
    public void SetUp(IAllureXunitRegistrationContext context)
    {
        context.TransformConfiguration(configuration => configuration.WithProperty(
            static configuration => configuration.GlobalLabels,
            configuration.Configuration.GlobalLabels.SetItem(
                "hook",
                configuration.Configuration.GlobalLabels.TryGetValue("hook", out var existing)
                    ? $"{existing} -> {value}"
                    : value
            ),
            static (configuration, labels) => configuration with
            {
                GlobalLabels = labels,
            }
        ));
    }
}

public sealed class EnvironmentRegistrationHook() : AllureRegistrationHook("environment");

public sealed class ConfigurationRegistrationHook() : AllureRegistrationHook("configuration");

static class AllureSetup
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        AllureXunitRegistrationHook.Current = new AllureRegistrationHook("programmatic");
    }
}

public class TestClass
{
    [Fact]
    public void TestMethod() { }
}
