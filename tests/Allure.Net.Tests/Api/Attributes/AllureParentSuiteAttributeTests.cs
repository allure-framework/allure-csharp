using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureParentSuiteAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureParentSuiteAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task ParentSuiteCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureParentSuiteAttribute("parent value");

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("parentSuite");
        await Assert.That(attr.Value).IsEqualTo("parent value");
        await AttributeAssertions.AssertLabels(tr, ("parentSuite", "parent value"));
    }

    [AllureParentSuite("base")]
    private class Base;
    [AllureParentSuite("derived")]
    private class Derived : Base;
}
