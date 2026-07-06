using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.BddLabels;

class BddLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.BddHierarchyAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckBddHierarchyOnTestMethodWorks()
    {
        var testResult = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.Tests.Samples.BddHierarchyAttributes.OnTestMethod.TestMethod"
        );
        await Assert.That(testResult).HasSingleLabel("epic").With.Value("Foo");
        await Assert.That(testResult).HasSingleLabel("feature").With.Value("Bar");
        await Assert.That(testResult).HasSingleLabel("story").With.Value("Baz");
    }

    [Test]
    public async Task CheckBddHierarchyOnTestClassWorks()
    {
        var testResult = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.Tests.Samples.BddHierarchyAttributes.OnTestClass.TestMethod"
        );
        await Assert.That(testResult).HasSingleLabel("epic").With.Value("Foo");
        await Assert.That(testResult).HasSingleLabel("feature").With.Value("Bar");
        await Assert.That(testResult).HasSingleLabel("story").With.Value("Baz");
    }

    [Test]
    public async Task CheckBddHierarchyOnBaseClassWorks()
    {
        var testResult = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.Tests.Samples.BddHierarchyAttributes.OnBaseClass.TestMethod"
        );
        await Assert.That(testResult).HasSingleLabel("epic").With.Value("Foo");
        await Assert.That(testResult).HasSingleLabel("feature").With.Value("Bar");
        await Assert.That(testResult).HasSingleLabel("story").With.Value("Baz");
    }

    [Test]
    public async Task CheckBddHierarchyOnInterfaceWorks()
    {
        var testResult = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.Tests.Samples.BddHierarchyAttributes.OnInterface.TestMethod"
        );
        await Assert.That(testResult).HasSingleLabel("epic").With.Value("Foo");
        await Assert.That(testResult).HasSingleLabel("feature").With.Value("Bar");
        await Assert.That(testResult).HasSingleLabel("story").With.Value("Baz");
    }
}
