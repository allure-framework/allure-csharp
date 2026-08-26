using Allure.Model;
using Allure.Sdk.Results;

using AllureTestResult = Allure.Model.TestResult;

namespace Allure.TestingPlatform.Tests.BindingTestExecution;

public class BindingTestExecutionTests
{
    [Test]
    public async Task ShouldRouteRuntimeOperationsThroughExecutionBinding()
    {
        var destination = await BindingExecutionTestApplication.RunAsync(async scenario =>
        {
            await scenario.BindAsync("node", "execution");
            await scenario.StartTestNodeAsync("node");
            await scenario.RunInExecutionAsync(
                "execution",
                () => UpdateCurrentTestAsync("updated")
            );
            await scenario.FinishExecutionAsync("execution");
            await scenario.FinishTestNodeAsync("node");
        });

        var result = await Assert.That(destination.TestResults).HasSingleItem();
        await Assert.That(result.Name).IsEqualTo("updated");
        await AssertMarker(result, "updated");
    }

    [Test]
    public async Task ShouldBufferRuntimeOperationsUntilBindingIsEstablished()
    {
        var destination = await BindingExecutionTestApplication.RunAsync(async scenario =>
        {
            await scenario.RunInExecutionAsync(
                "execution",
                () => UpdateCurrentTestAsync("buffered")
            );
            await scenario.StartTestNodeAsync("node");
            await scenario.FinishTestNodeAsync("node");
            await scenario.BindAsync("node", "execution");
            await scenario.FinishExecutionAsync("execution");
        });

        var result = await Assert.That(destination.TestResults).HasSingleItem();
        await Assert.That(result.Name).IsEqualTo("buffered");
        await AssertMarker(result, "buffered");
    }

    [Test]
    public async Task ShouldWaitForBothTestNodeAndExecutionToFinish()
    {
        var nodeFinishesFirst = await BindingExecutionTestApplication.RunAsync(async scenario =>
        {
            await scenario.StartTestNodeAsync("node");
            await scenario.RunInExecutionAsync(
                "execution",
                () => UpdateCurrentTestAsync("node-first")
            );
            await scenario.FinishTestNodeAsync("node");
            await scenario.BindAsync("node", "execution");
        });

        var executionFinishesFirst = await BindingExecutionTestApplication.RunAsync(
            async scenario =>
            {
                await scenario.BindAsync("node", "execution");
                await scenario.RunInExecutionAsync(
                    "execution",
                    () => UpdateCurrentTestAsync("execution-first")
                );
                await scenario.FinishExecutionAsync("execution");
                await scenario.StartTestNodeAsync("node");
            },
            allowNoCompletedTestNodes: true
        );

        await Assert.That(nodeFinishesFirst.TestResults).IsEmpty();
        await Assert.That(executionFinishesFirst.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldHandleTestNodeFinishWithoutStart()
    {
        var destination = await BindingExecutionTestApplication.RunAsync(async scenario =>
        {
            await scenario.RunInExecutionAsync(
                "execution",
                () => UpdateCurrentTestAsync("unpaired")
            );
            await scenario.FinishTestNodeAsync("node");
            await scenario.BindAsync("node", "execution");
            await scenario.FinishExecutionAsync("execution");
        });

        var result = await Assert.That(destination.TestResults).HasSingleItem();
        await Assert.That(result.Name).IsEqualTo("unpaired");
        await AssertMarker(result, "unpaired");
    }

    [Test]
    public async Task ShouldIsolateExecutionsThatReuseTestNodeUid()
    {
        var destination = await BindingExecutionTestApplication.RunAsync(async scenario =>
        {
            await scenario.BindAsync("node", "execution-1");
            await scenario.StartTestNodeAsync("node", "first node occurrence");
            await scenario.RunInExecutionAsync(
                "execution-1",
                () => UpdateCurrentTestAsync("first")
            );
            await scenario.FinishTestNodeAsync("node", "first node occurrence");

            await scenario.BindAsync("node", "execution-2");
            await scenario.StartTestNodeAsync("node", "second node occurrence");
            await scenario.RunInExecutionAsync(
                "execution-2",
                () => UpdateCurrentTestAsync("second")
            );
            await scenario.FinishTestNodeAsync("node", "second node occurrence");

            await scenario.FinishExecutionAsync("execution-1");
            await scenario.FinishExecutionAsync("execution-2");
        });

        await Assert.That(destination.TestResults).Count().IsEqualTo(2);
        await AssertResultMarkers(destination, "first", "second");
    }

    [Test]
    public async Task ShouldPairLateBindingsWithReusedTestNodeOccurrencesInOrder()
    {
        var destination = await BindingExecutionTestApplication.RunAsync(async scenario =>
        {
            await scenario.StartTestNodeAsync("node", "first node occurrence");
            await scenario.RunInExecutionAsync(
                "execution-1",
                () => UpdateCurrentTestAsync("first")
            );
            await scenario.FinishTestNodeAsync("node", "first node occurrence");

            await scenario.StartTestNodeAsync("node", "second node occurrence");
            await scenario.RunInExecutionAsync(
                "execution-2",
                () => UpdateCurrentTestAsync("second")
            );
            await scenario.FinishTestNodeAsync("node", "second node occurrence");

            await scenario.BindAsync("node", "execution-1");
            await scenario.FinishExecutionAsync("execution-1");
            await scenario.BindAsync("node", "execution-2");
            await scenario.FinishExecutionAsync("execution-2");
        });

        await Assert.That(destination.TestResults).Count().IsEqualTo(2);
        await AssertResultMarkers(destination, "first", "second");
    }

    [Test]
    public async Task ShouldCoordinateDifferentTestNodeUidsIndependently()
    {
        var destination = await BindingExecutionTestApplication.RunAsync(async scenario =>
        {
            await scenario.StartTestNodeAsync("node-1");
            await scenario.StartTestNodeAsync("node-2");
            await scenario.RunInExecutionAsync(
                "execution-1",
                () => UpdateCurrentTestAsync("first")
            );
            await scenario.RunInExecutionAsync(
                "execution-2",
                () => UpdateCurrentTestAsync("second")
            );

            await scenario.FinishTestNodeAsync("node-2");
            await scenario.BindAsync("node-2", "execution-2");
            await scenario.FinishExecutionAsync("execution-2");

            await scenario.FinishTestNodeAsync("node-1");
            await scenario.BindAsync("node-1", "execution-1");
            await scenario.FinishExecutionAsync("execution-1");
        });

        await Assert.That(destination.TestResults).Count().IsEqualTo(2);
        await AssertResultMarkers(destination, "first", "second");
    }

    static async Task UpdateCurrentTestAsync(string marker)
    {
        await AllureApi.SetTestNameAsync(marker, CancellationToken.None);
        await AllureApi.AddLabelAsync(
            Label.Create("execution-marker", marker),
            CancellationToken.None
        );
    }

    static async Task AssertMarker(AllureTestResult result, string expected)
    {
        var marker = await Assert.That(
            result.Labels.Where(label => label.Name == "execution-marker")
        ).HasSingleItem();
        await Assert.That(marker.Value).IsEqualTo(expected);
    }

    static async Task AssertResultMarkers(
        InMemoryResultsDestination destination,
        params string[] expected
    )
    {
        var markers = destination.TestResults
            .SelectMany(result => result.Labels)
            .Where(label => label.Name == "execution-marker")
            .Select(label => label.Value)
            .ToArray();

        await Assert.That(markers).IsEquivalentTo(expected);
        await Assert.That(destination.TestResults.Select(result => result.Name))
            .IsEquivalentTo(expected);
    }
}
