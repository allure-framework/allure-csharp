using System.Reflection;
using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class FacadeCompletenessTests
{
    [Test]
    public async Task EveryAllureApiMethodDispatchesExactlyOneOperation()
    {
        var currentSync = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        var currentAsync = RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();
        var globalSync = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        var globalAsync = RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(
            new TestApiEndpoint(currentSync.Instance, currentAsync.Instance),
            new TestApiEndpoint(globalSync.Instance, globalAsync.Instance)
        );
        var routingFailures = new List<string>();

        foreach (var definition in PublicMethods(typeof(AllureApi)))
        {
            var method = PublicMethodArguments.Close(definition);
            var recorders = new[] { currentSync.Calls, currentAsync.Calls, globalSync.Calls, globalAsync.Calls };
            var before = recorders.Select(calls => calls.Count).ToArray();

            try
            {
                var result = method.Invoke(null, PublicMethodArguments.Create(method));
                if (result is Task task)
                {
                    await task;
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Failed to invoke {method}.", exception);
            }

            var deltas = recorders.Select((calls, index) => calls.Count - before[index]).ToArray();
            var expectedRecorder = method.Name.Contains("Global", StringComparison.Ordinal)
                ? method.Name.EndsWith("Async", StringComparison.Ordinal) ? 3 : 2
                : method.Name.EndsWith("Async", StringComparison.Ordinal) ? 1 : 0;

            if (deltas.Sum() != 1 || deltas[expectedRecorder] != 1)
            {
                routingFailures.Add(method.ToString()!);
            }
        }

        await Assert.That(routingFailures).IsEmpty()
            .Because("every method must dispatch once through the correct scope and operation channel");
    }

    [Test]
    public async Task EveryInProcessApiMethodDispatchesExactlyOneOperation()
    {
        var operations = RecordingInterface<IAllureInProcessOperations>.Create();
        operations.Handler = (method, arguments) =>
        {
            if (method.Name.StartsWith("TryRead", StringComparison.Ordinal))
            {
                arguments[1] = "read value";
                return true;
            }
            return null;
        };
        using var scope = FacadeTestEnvironment.Use(
            current: new TestApiEndpoint(sync: operations.Instance)
        );

        foreach (var definition in PublicMethods(typeof(AllureInProcessApi)))
        {
            var method = PublicMethodArguments.Close(definition);
            var before = operations.Calls.Count;

            try
            {
                _ = method.Invoke(null, PublicMethodArguments.Create(method));
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Failed to invoke {method}.", exception);
            }

            await Assert.That(operations.Calls.Count - before)
                .IsEqualTo(1)
                .Because($"{method} must dispatch exactly one runtime operation");
        }
    }

    static IEnumerable<MethodInfo> PublicMethods(Type facade) =>
        facade.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => !method.IsSpecialName)
            .OrderBy(method => method.Name)
            .ThenBy(method => method.ToString());
}
