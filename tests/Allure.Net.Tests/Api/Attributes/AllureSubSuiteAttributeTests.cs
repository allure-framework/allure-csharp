using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureSubSuiteAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureSubSuiteAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task SubSuiteCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureSubSuiteAttribute("sub value");

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("subSuite");
        await Assert.That(attr.Value).IsEqualTo("sub value");
        await AttributeAssertions.AssertLabels(tr, ("subSuite", "sub value"));
    }

    [AllureSubSuite("base")]
    private class Base;
    [AllureSubSuite("derived")]
    private class Derived : Base;
}
