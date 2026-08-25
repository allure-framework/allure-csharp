using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Names;

class NameTests
{
    const string SampleNamespace = "Allure.Xunit.v3.Tests.Samples.Names.NameApi";

    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.NameApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(16);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task ShouldRenameFactResultViaAllureName()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Lorem Ipsum on FactMethodRenamedInAllure"
        ).With.FullName(FullName("AttributeTestClass", "FactMethodRenamedInAllure"));
    }

    [Test]
    public async Task ShouldRenameTheoryResultViaAllureName()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Lorem Ipsum on TheoryMethodRenamedInAllure"
        ).With.FullName(
            FullName(
                "AttributeTestClass",
                "TheoryMethodRenamedInAllure",
                "System.String"
            )
        );
    }

    [Test]
    public async Task ShouldRenameFactResultViaXunitDisplayName()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Lorem Ipsum on FactMethodRenamedInXunit"
        ).With.FullName(FullName("AttributeTestClass", "FactMethodRenamedInXunit"));
    }

    [Test]
    public async Task ShouldRenameTheoryResultViaXunitDisplayName()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Lorem Ipsum on TheoryMethodRenamedInXunit(value: \"foo\")"
        ).With.FullName(
            FullName(
                "AttributeTestClass",
                "TheoryMethodRenamedInXunit",
                "System.String"
            )
        );
    }

    [Test]
    [Arguments("SyncSetTestName", "Sync test name")]
    [Arguments("AsyncSetTestName", "Async test name")]
    public async Task SetTestNameCallsWork(string className, string newName)
    {
        await Assert.That(results.Value).HasSingleTestResult(newName)
            .With.Status(AllureStatus.Passed)
            .With.FullName(FullName(className, "TestMethod"));
    }

    [Test]
    [Arguments("SyncSetFixtureName", "Sync fixture name")]
    [Arguments("AsyncSetFixtureName", "Async fixture name")]
    public async Task SetFixtureNameCallsWork(string className, string fixtureName)
    {
        await AssertRenamedFixture(className, fixtureName);
    }

    [Test]
    [Arguments("SyncSetNameOnTest", "Sync test name via SetName")]
    [Arguments("AsyncSetNameOnTest", "Async test name via SetName")]
    public async Task SetNameFallsBackToTest(string className, string newName)
    {
        await Assert.That(results.Value).HasSingleTestResult(newName)
            .With.Status(AllureStatus.Passed)
            .With.FullName(FullName(className, "TestMethod"));
    }

    [Test]
    [Arguments("SyncSetNameOnFixture", "Sync fixture name via SetName")]
    [Arguments("AsyncSetNameOnFixture", "Async fixture name via SetName")]
    public async Task SetNamePrioritizesFixtureOverTest(
        string className,
        string fixtureName
    )
    {
        await AssertRenamedFixture(className, fixtureName);
    }

    [Test]
    [Arguments("SyncSetNameOnStep", "Sync step name via SetName")]
    [Arguments("AsyncSetNameOnStep", "Async step name via SetName")]
    public async Task SetNamePrioritizesStepOverTest(
        string className,
        string stepName
    )
    {
        var uuid = await Assert.That(results.Value).HasSingleTestResult(TestName(className))
            .With.Status(AllureStatus.Passed)
            .With.StepsMatching([])
            .With.Uuid();

        await Assert.That(results.Value).HasSingleContainer(container => container
            .HasChildrenMatching([child => child.IsEqualTo(uuid)]))
            .With.OnlyOneSetUpFixture(fixture => fixture
                .HasName("Original fixture")
                .And.HasStatus(AllureStatus.Passed)
                .And.HasStepsMatching([
                    step => step.HasName(stepName)
                        .And.HasStatus(AllureStatus.Passed)
                        .And.HasParametersMatching([])
                        .And.HasStepsMatching([]),
                ]))
            .With.AftersMatching([])
            .With.SingleChild().That.IsEqualTo(uuid);
    }

    [Test]
    public async Task StaticXunitTestsAreSupported()
    {
        await Assert.That(results.Value).HasSingleTestResult(TestName("StaticFact"))
            .With.Status(AllureStatus.Passed)
            .With.FullName(FullName("StaticFact", "TestMethod"));
    }

    static async Task AssertRenamedFixture(string className, string fixtureName)
    {
        var testName = TestName(className);
        var uuid = await Assert.That(results.Value).HasSingleTestResult(testName)
            .With.Status(AllureStatus.Passed)
            .With.Uuid();

        await Assert.That(results.Value).HasSingleContainer(container => container
            .HasChildrenMatching([child => child.IsEqualTo(uuid)]))
            .With.OnlyOneSetUpFixture(fixture => fixture
                .HasName(fixtureName)
                .And.HasStatus(AllureStatus.Passed))
            .With.AftersMatching([])
            .With.SingleChild().That.IsEqualTo(uuid);
    }

    static string TestName(string className) =>
        $"{SampleNamespace}.{className}.TestMethod";

    static string FullName(
        string className,
        string methodName,
        string parameters = ""
    ) => $"{SampleNamespace}:{SampleNamespace}.{className}.{methodName}({parameters})";
}
