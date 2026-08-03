using System.Reflection;
using Allure.Aspects;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Concurrency;

public class AspectIsolationTests
{
    [Test]
    public async Task ParallelAspectsUseSerializerFromTheirOwnEndpoint()
    {
        var method = typeof(AspectIsolationTests).GetMethod(
            nameof(IsolatedStep),
            BindingFlags.Static | BindingFlags.NonPublic
        )!;

        var executions = Enumerable.Range(0, 24).Select(index => Task.Run(async () =>
        {
            var operations = new ExecutingOperations();
            using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint(
                new TestParameterSerializer($"scope-{index}")
            ));

            await Task.Yield();
            var result = new AllureStepAspect().Around(
                nameof(IsolatedStep),
                [index],
                _ => index,
                method,
                typeof(int)
            );
            await Task.Yield();

            return new
            {
                Index = index,
                Result = (int)result!,
                Call = operations.Sync.SingleCall,
            };
        }));

        var results = await Task.WhenAll(executions);

        foreach (var result in results)
        {
            await Assert.That(result.Result).IsEqualTo(result.Index);
            await Assert.That(result.Call.Arguments[0])
                .IsEqualTo($"isolated scope-{result.Index}:{result.Index}");
        }
    }

    [AllureStep("isolated {value}")]
    static int IsolatedStep(int value) => value;
}
