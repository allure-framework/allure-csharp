using Allure.Testing;

namespace Allure.Xunit.v3.Tests.RegistrationHooks;

class RegistrationHookTests
{
    [Test]
    public async Task ShouldApplyProgrammaticRegistrationHook(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.ProgrammaticHook, token);

        var testResult = await Assert.That(results).HasSingleTestResult();

        await Assert.That(testResult).HasSingleLabel("hook").With.Value("true");
    }

    [Test]
    public async Task ShouldApplyConfigurationRegistrationHook(CancellationToken token)
    {
        var hookAssembly = "Allure.Xunit.v3.Tests.Samples.RegistrationHooks.ReflectionHook";
        var hookAssemblyQualifiedName = $"{hookAssembly}.AllureRegistrationHook, {hookAssembly}";

        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.ReflectionHook,
            new()
            {
                AllureConfiguration = new
                {
                    runtimeRegistrationHook = hookAssemblyQualifiedName
                },
            },
            token
        );

        var testResult = await Assert.That(results).HasSingleTestResult();

        await Assert.That(testResult).HasSingleLabel("hook").With.Value("true");
    }

    [Test]
    public async Task ShouldApplyEnvironmentRegistrationHook(CancellationToken token)
    {
        var hookAssembly = "Allure.Xunit.v3.Tests.Samples.RegistrationHooks.ReflectionHook";
        var hookAssemblyQualifiedName = $"{hookAssembly}.AllureRegistrationHook, {hookAssembly}";

        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.ReflectionHook,
            new()
            {
                EnvironmentVariables =
                {
                    ["ALLURE_XUNIT_REGISTRATION_HOOK"] = hookAssemblyQualifiedName
                },
            },
            token
        );

        var testResult = await Assert.That(results).HasSingleTestResult();

        await Assert.That(testResult).HasSingleLabel("hook").With.Value("true");
    }

    [Test]
    public async Task ShouldApplyMultipleRegistrationHooksInOrder(CancellationToken token)
    {
        var hookAssembly = "Allure.Xunit.v3.Tests.Samples.RegistrationHooks.MultipleHooks";
        var environmentHook = $"{hookAssembly}.EnvironmentRegistrationHook, {hookAssembly}";
        var configurationHook = $"{hookAssembly}.ConfigurationRegistrationHook, {hookAssembly}";

        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.MultipleHooks,
            new()
            {
                AllureConfiguration = new
                {
                    runtimeRegistrationHook = configurationHook
                },
                EnvironmentVariables =
                {
                    ["ALLURE_XUNIT_REGISTRATION_HOOK"] = environmentHook
                },
            },
            token
        );

        var testResult = await Assert.That(results).HasSingleTestResult();

        await Assert.That(testResult).HasSingleLabel("hook")
            .With.Value("environment -> configuration -> programmatic");
    }
}
