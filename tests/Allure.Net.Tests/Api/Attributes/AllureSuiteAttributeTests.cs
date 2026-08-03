using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureSuiteAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureSuiteAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task SuiteCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureSuiteAttribute("suite value");

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("suite");
        await Assert.That(attr.Value).IsEqualTo("suite value");
        await AttributeAssertions.AssertLabels(tr, ("suite", "suite value"));
    }

    [AllureSuite("base")]
    private class Base;
    [AllureSuite("derived")]
    private class Derived : Base;
}
