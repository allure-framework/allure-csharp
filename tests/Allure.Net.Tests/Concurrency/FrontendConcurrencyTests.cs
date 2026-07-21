using System.Collections.Concurrent;
using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;
using Allure.Runtime;

namespace Allure.Net.Tests.Concurrency;

public class FrontendConcurrencyTests
{
    [Test]
    public async Task ExactlyOneConcurrentPreparationSucceeds()
    {
        var state = new AllureFrontendState(new TestApiClient("default"));
        var clients = Enumerable.Range(0, 32)
            .Select(index => new TestApiClient($"client-{index}"))
            .ToArray();
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var successes = new ConcurrentBag<TestApiClient>();
        var failures = new ConcurrentBag<Exception>();

        var attempts = clients.Select(client => Task.Run(async () =>
        {
            await start.Task;
            try
            {
                state.PrepareClient(client);
                successes.Add(client);
            }
            catch (Exception exception)
            {
                failures.Add(exception);
            }
        })).ToArray();
        start.SetResult();
        await Task.WhenAll(attempts);

        await Assert.That(successes.Count).IsEqualTo(1);
        await Assert.That(failures.Count).IsEqualTo(clients.Length - 1);
        await Assert.That(failures.All(error => error is InvalidOperationException)).IsTrue();
        await Assert.That(state.Client).IsSameReferenceAs(successes.Single());
    }

    [Test]
    public async Task ConcurrentReadAndPreparationPublishOneConsistentClient()
    {
        for (var iteration = 0; iteration < 100; iteration++)
        {
            var defaultClient = new TestApiClient("default");
            var preparedClient = new TestApiClient("prepared");
            var state = new AllureFrontendState(defaultClient);
            var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            IAllureApiClient? observed = null;
            Exception? preparationError = null;

            var reader = Task.Run(async () =>
            {
                await start.Task;
                observed = state.Client;
            });
            var preparer = Task.Run(async () =>
            {
                await start.Task;
                try
                {
                    state.PrepareClient(preparedClient);
                }
                catch (Exception exception)
                {
                    preparationError = exception;
                }
            });

            start.SetResult();
            await Task.WhenAll(reader, preparer);

            if (preparationError is null)
            {
                await Assert.That(observed).IsSameReferenceAs(preparedClient);
            }
            else
            {
                await Assert.That(preparationError).IsTypeOf<InvalidOperationException>();
                await Assert.That(observed).IsSameReferenceAs(defaultClient);
            }

            await Assert.That(state.Client).IsSameReferenceAs(observed);
        }
    }
}
