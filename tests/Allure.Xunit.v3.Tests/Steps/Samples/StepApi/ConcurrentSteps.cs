using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Steps.StepApi
{
    public class ConcurrentAllureApiSteps
    {
        const int StepCount = 5;

        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            var barrier = new AsyncBarrier(StepCount);

            await Task.WhenAll(
                Enumerable.Range(1, StepCount).Select(index =>
                    AllureApi.StepAsync(
                        $"Concurrent step {index}",
                        () => barrier.SignalAndWait(token),
                        token
                    )
                )
            );
        }
    }

    public class NestedConcurrentAllureApiSteps
    {
        const int ParentCount = 3;
        const int ChildCount = 3;

        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            var barrier = new AsyncBarrier(ParentCount * ChildCount);

            await Task.WhenAll(
                Enumerable.Range(1, ParentCount).Select(parent =>
                    AllureApi.StepAsync(
                        $"Parent {parent}",
                        () => Task.WhenAll(
                            Enumerable.Range(1, ChildCount).Select(child =>
                                AllureApi.StepAsync(
                                    $"Child {parent}.{child}",
                                    () => barrier.SignalAndWait(token),
                                    token
                                )
                            )
                        ),
                        token
                    )
                )
            );
        }
    }

    sealed class AsyncBarrier
    {
        readonly int participantCount;
        readonly TaskCompletionSource release = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        int participants;

        public AsyncBarrier(int participantCount)
        {
            this.participantCount = participantCount;
        }

        public async Task SignalAndWait(CancellationToken token)
        {
            if (Interlocked.Increment(ref this.participants) == this.participantCount)
            {
                this.release.SetResult();
            }

            await this.release.Task.WaitAsync(TimeSpan.FromSeconds(10), token);
        }
    }
}
