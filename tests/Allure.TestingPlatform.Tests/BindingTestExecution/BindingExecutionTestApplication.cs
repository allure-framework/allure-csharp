using Allure.Sdk.Results;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;

namespace Allure.TestingPlatform.Tests.BindingTestExecution;

static class BindingExecutionTestApplication
{
    static readonly string[] DefaultArgs =
    [
        "--no-progress",
        "--no-ansi",
        "--output",
        "Normal",
        "--show-stdout",
        "None",
        "--show-stderr",
        "None",
    ];

    public static async Task<InMemoryResultsDestination> RunAsync(
        Func<BindingExecutionScenario, Task> runScenario,
        bool allowNoCompletedTestNodes = false
    )
    {
        var destination = new InMemoryResultsDestination();
        var executionContext = new BindingExecutionContext();
        var builder = await TestApplication.CreateBuilderAsync(DefaultArgs);

        builder.AddEmbeddedAllure(
            $"binding-execution-test-{Guid.NewGuid():N}",
            (context, _) =>
            {
                context.UseConfiguration(new AllureTestingPlatformConfiguration
                {
                    IsProcessWatchdogEnabled = false,
                });
                context.UseCorrelationContext(_ => executionContext);
                context.UseCorrelationStrategy(_ => executionContext);
                context.UseExecutionStateContext(_ => executionContext);
                context.UseDestination(_ => destination);
                context.UseBindingTestExecutionCoordinator();
            },
            (context, _, _) =>
            {
                context.UseCurrentScopePredicate(_ => executionContext.IsActive);
                context.UseGlobalScopePredicate(_ => executionContext.IsActive);
            }
        );
        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, _) => new BindingExecutionTestFramework(
                executionContext,
                runScenario
            )
        );

        using var app = await builder.BuildAsync();
        var exitCode = await app.RunAsync();
        if (exitCode != 0 && !(allowNoCompletedTestNodes && exitCode == 8))
        {
            throw new InvalidOperationException(
                $"The binding execution test application exited with code {exitCode}."
            );
        }

        return destination;
    }
}
