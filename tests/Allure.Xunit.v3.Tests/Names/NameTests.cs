using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Names;

class NameTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.RenamedTestsAndClasses, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(5);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task ShouldRenameFactResultViaAllureName()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Lorem Ipsum on FactMethodRenamedInAllure"
        ).With.FullName(
            "Allure.Xunit.v3.Tests.Samples.Names.RenamedTestsAndClasses:"
                + "Allure.Xunit.v3.Tests.Samples.Names.RenamedTestsAndClasses."
                + "TestClass.FactMethodRenamedInAllure()"
        );
    }

    [Test]
    public async Task ShouldRenameTheoryResultViaAllureName()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Lorem Ipsum on TheoryMethodRenamedInAllure"
        ).With.FullName(
            "Allure.Xunit.v3.Tests.Samples.Names.RenamedTestsAndClasses:"
                + "Allure.Xunit.v3.Tests.Samples.Names.RenamedTestsAndClasses."
                + "TestClass.TheoryMethodRenamedInAllure(System.String)"
        );
    }

    [Test]
    public async Task ShouldRenameFactResultViaXunitDisplayName()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Lorem Ipsum on FactMethodRenamedInXunit"
        ).With.FullName(
            "Allure.Xunit.v3.Tests.Samples.Names.RenamedTestsAndClasses:"
                + "Allure.Xunit.v3.Tests.Samples.Names.RenamedTestsAndClasses."
                + "TestClass.FactMethodRenamedInXunit()"
        );
    }

    [Test]
    public async Task ShouldRenameTheoryResultViaXunitDisplayName()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Lorem Ipsum on TheoryMethodRenamedInXunit(value: \"foo\")"
        ).With.FullName(
            "Allure.Xunit.v3.Tests.Samples.Names.RenamedTestsAndClasses:"
                + "Allure.Xunit.v3.Tests.Samples.Names.RenamedTestsAndClasses."
                + "TestClass.TheoryMethodRenamedInXunit(System.String)"
        );
    }
}
