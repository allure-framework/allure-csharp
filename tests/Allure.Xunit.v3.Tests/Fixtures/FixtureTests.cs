using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Fixtures;

class FixtureTests
{
    const string SampleNamespace = "Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi";

    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.FixtureApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(13);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task SetUpOnConstructorAndTearDownOnDisposeWork()
    {
        var fullName = FullName("AttributeOnConstructorAndDispose");
        var uuid = await Assert.That(results.Value).HasSingleTestResult(fullName).With.Uuid();

        await Assert.That(results.Value).HasSingleContainer(container => container
            .HasChildrenMatching([child => child.IsEqualTo(uuid)]))
            .With.OnlyOneSetUpFixture(fixture => fixture
                .HasName("Constructor setup")
                .And.HasStatus(AllureStatus.Passed))
            .With.OnlyOneTearDownFixture(fixture => fixture
                .HasName("Dispose teardown")
                .And.HasStatus(AllureStatus.Passed))
            .With.SingleChild().That.IsEqualTo(uuid);
    }

    [Test]
    public async Task SetUpAndTearDownOnAsyncLifetimeWork()
    {
        var fullName = FullName("AttributeOnAsyncLifetime");
        var uuid = await Assert.That(results.Value).HasSingleTestResult(fullName).With.Uuid();

        await Assert.That(results.Value).HasSingleContainer(container => container
            .HasChildrenMatching([child => child.IsEqualTo(uuid)]))
            .With.OnlyOneSetUpFixture(fixture => fixture
                .HasName("InitializeAsync setup")
                .And.HasStatus(AllureStatus.Passed))
            .With.OnlyOneTearDownFixture(fixture => fixture
                .HasName("IAsyncLifetime teardown")
                .And.HasStatus(AllureStatus.Passed))
            .With.SingleChild().That.IsEqualTo(uuid);
    }

    [Test]
    public async Task TearDownOnAsyncDisposableWorks()
    {
        var fullName = FullName("AttributeOnAsyncDisposable");
        var uuid = await Assert.That(results.Value).HasSingleTestResult(fullName).With.Uuid();

        await Assert.That(results.Value).HasSingleContainer(container => container
            .HasChildrenMatching([child => child.IsEqualTo(uuid)]))
            .With.BeforesMatching([])
            .With.OnlyOneTearDownFixture(fixture => fixture
                .HasName("IAsyncDisposable teardown")
                .And.HasStatus(AllureStatus.Passed))
            .With.SingleChild().That.IsEqualTo(uuid);
    }

    [Test]
    public async Task SyncAllureApiFixtureCallsAndContextsWork() =>
        await AssertPassedRuntimeFixtures(
            "AllureApiSyncCallsFromLifecycle",
            "AllureApi sync setup",
            "AllureApi sync teardown"
        );

    [Test]
    public async Task AsyncAllureApiFixtureCallsAndContextsWork() =>
        await AssertPassedRuntimeFixtures(
            "AllureApiAsyncCallsFromLifecycle",
            "AllureApi async setup",
            "AllureApi async teardown"
        );

    [Test]
    public async Task SyncAllureInProcessApiFixtureCallsAndContextsWork() =>
        await AssertPassedRuntimeFixtures(
            "AllureInProcessApiSyncCallsFromLifecycle",
            "AllureInProcessApi sync setup",
            "AllureInProcessApi sync teardown"
        );

    [Test]
    public async Task AsyncAllureInProcessApiFixtureCallsAndContextsWork() =>
        await AssertPassedRuntimeFixtures(
            "AllureInProcessApiAsyncCallsFromLifecycle",
            "AllureInProcessApi async setup",
            "AllureInProcessApi async teardown"
        );

    [Test]
    public async Task NestedSyncAllureApiFixturesAreNotAllowed() =>
        await AssertNestedFixtureRejected(
            "NestedAllureApiSyncFixtures",
            "Outer AllureApi sync fixture"
        );

    [Test]
    public async Task NestedAsyncAllureApiFixturesAreNotAllowed() =>
        await AssertNestedFixtureRejected(
            "NestedAllureApiAsyncFixtures",
            "Outer AllureApi async fixture"
        );

    [Test]
    public async Task NestedSyncAllureInProcessApiFixturesAreNotAllowed() =>
        await AssertNestedFixtureRejected(
            "NestedAllureInProcessApiSyncFixtures",
            "Outer AllureInProcessApi sync fixture"
        );

    [Test]
    public async Task NestedAsyncAllureInProcessApiFixturesAreNotAllowed() =>
        await AssertNestedFixtureRejected(
            "NestedAllureInProcessApiAsyncFixtures",
            "Outer AllureInProcessApi async fixture"
        );

    [Test]
    public async Task CtorDisposeFixturesAreTestScoped()
    {
        var test1Uuid = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi.TheoryFixture.TestMethod(_: 1)"
        ).With.Uuid();
        var test2Uuid = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Fixtures.FixtureApi.TheoryFixture.TestMethod(_: 2)"
        ).With.Uuid();

        var test1Container = await Assert.That(results.Value).HasSingleContainer(
            (tc) => tc.HasSingleChild().That.IsEqualTo(test1Uuid)
        );
        await Assert.That(test1Container).HasSingleSetUpFixture().That.HasName("Set up");
        await Assert.That(test1Container).HasSingleTearDownFixture().That.HasName("Tear down");

        var test2Container = await Assert.That(results.Value).HasSingleContainer(
            (tc) => tc.HasSingleChild().That.IsEqualTo(test2Uuid)
        );
        await Assert.That(test2Container).HasSingleSetUpFixture().That.HasName("Set up");
        await Assert.That(test2Container).HasSingleTearDownFixture().That.HasName("Tear down");
    }

    static async Task AssertPassedRuntimeFixtures(
        string className,
        string setUpName,
        string tearDownName
    )
    {
        var fullName = FullName(className);
        var uuid = await Assert.That(results.Value).HasSingleTestResult(fullName).With.Uuid();

        await Assert.That(results.Value).HasSingleContainer(container => container
            .HasChildrenMatching([child => child.IsEqualTo(uuid)]))
            .With.OnlyOneSetUpFixture(fixture => fixture
                .HasName(setUpName)
                .And.HasStatus(AllureStatus.Passed)
                .And.HasParametersMatching([
                    parameter => parameter.HasName("context").And.HasValue("works"),
                ]))
            .With.OnlyOneTearDownFixture(fixture => fixture
                .HasName(tearDownName)
                .And.HasStatus(AllureStatus.Passed)
                .And.HasParametersMatching([
                    parameter => parameter.HasName("context").And.HasValue("works"),
                ]))
            .With.SingleChild().That.IsEqualTo(uuid);
    }

    static async Task AssertNestedFixtureRejected(string className, string fixtureName)
    {
        var fullName = FullName(className);
        var uuid = await Assert.That(results.Value).HasSingleTestResult(fullName)
            .With.Status(AllureStatus.Passed)
            .With.Uuid();

        await Assert.That(results.Value).HasSingleContainer(container => container
            .HasChildrenMatching([child => child.IsEqualTo(uuid)]))
            .With.BeforesMatching([
                fixture => fixture
                    .HasName(fixtureName)
                    .And.HasStatus(AllureStatus.Passed),
                fixture => fixture
                    .HasName("Inner fixture")
                    .And.HasStatus(AllureStatus.Broken)
                    .And.HasStatusDetails(details => details
                        .HasMessage(message => message.Contains("Another fixture is currently running."))),
            ])
            .With.AftersMatching([])
            .With.SingleChild().That.IsEqualTo(uuid);
    }

    static string FullName(string className) =>
        $"{SampleNamespace}.{className}.TestMethod";
}
