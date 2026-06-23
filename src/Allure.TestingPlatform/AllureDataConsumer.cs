using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Allure.TestingPlatform;

/// <summary>
/// Consumes Microsoft.Testing.Platform <see cref="TestNodeUpdateMessage"/>s
/// and projects them onto Allure's <see cref="AllureLifecycle"/>.
/// </summary>
/// <remarks>
/// Minimal viable scope: maps terminal test states (Passed / Failed / Skipped
/// / Error / Timeout / Cancelled) to a single Schedule → Start → Update →
/// Stop → Write sequence per test. <c>InProgress</c> updates are ignored —
/// timing therefore reflects when the consumer processed the terminal
/// message, not the test's actual wall-clock duration. This will be addressed
/// in a follow-up PR by reading <see cref="TimingProperty"/>.
/// </remarks>
public sealed class AllureDataConsumer : IDataConsumer
{
    static readonly Type[] s_consumed = [typeof(TestNodeUpdateMessage)];

    readonly AllureLifecycle _lifecycle;
    readonly object _sequenceLock = new();

    public AllureDataConsumer() : this(AllureLifecycle.Instance) { }

    internal AllureDataConsumer(AllureLifecycle lifecycle)
    {
        _lifecycle = lifecycle;
    }

    public string Uid => "Allure.TestingPlatform";

    public string Version => typeof(AllureDataConsumer).Assembly.GetName().Version?.ToString() ?? "0.0.0";

    public string DisplayName => "Allure Report";

    public string Description => "Writes Allure result files for tests reported through Microsoft.Testing.Platform.";

    public Type[] DataTypesConsumed => s_consumed;

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public Task ConsumeAsync(
        IDataProducer dataProducer,
        IData value,
        CancellationToken cancellationToken)
    {
        if (value is TestNodeUpdateMessage update)
        {
            HandleUpdate(update);
        }
        return Task.CompletedTask;
    }

    void HandleUpdate(TestNodeUpdateMessage update)
    {
        var node = update.TestNode;
        var state = node.Properties.OfType<TestNodeStateProperty>().LastOrDefault();
        if (state is null || state is InProgressTestNodeStateProperty)
        {
            return;
        }

        var (status, message, trace) = MapState(state);

        var testResult = new TestResult
        {
            uuid = IdFunctions.CreateUUID(),
            name = node.DisplayName,
            fullName = node.Uid.Value,
            historyId = node.Uid.Value,
        };

        lock (_sequenceLock)
        {
            _lifecycle.StartTestCase(testResult);
            _lifecycle.UpdateTestCase(tr =>
            {
                tr.status = status;
                if (message is not null || trace is not null)
                {
                    tr.statusDetails ??= new StatusDetails();
                    tr.statusDetails.message = message;
                    tr.statusDetails.trace = trace;
                }
            });
            _lifecycle.StopTestCase();
            _lifecycle.WriteTestCase();
        }
    }

    static (Status status, string? message, string? trace) MapState(TestNodeStateProperty state) =>
        state switch
        {
            PassedTestNodeStateProperty => (Status.passed, null, null),
            SkippedTestNodeStateProperty s => (Status.skipped, s.Explanation, null),
            FailedTestNodeStateProperty f => (Status.failed, f.Exception?.Message, f.Exception?.StackTrace),
            ErrorTestNodeStateProperty e => (Status.broken, e.Exception?.Message, e.Exception?.StackTrace),
            TimeoutTestNodeStateProperty t => (Status.broken, t.Exception?.Message ?? "Test timed out", t.Exception?.StackTrace),
            CancelledTestNodeStateProperty c => (Status.broken, c.Exception?.Message ?? "Test cancelled", c.Exception?.StackTrace),
            _ => (Status.none, null, null),
        };
}
