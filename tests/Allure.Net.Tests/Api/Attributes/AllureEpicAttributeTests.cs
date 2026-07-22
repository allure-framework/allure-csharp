using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureEpicAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureEpicAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task EpicCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureEpicAttribute("epic value");

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("epic");
        await Assert.That(attr.Value).IsEqualTo("epic value");
        await AttributeAssertions.AssertLabels(tr, ("epic", "epic value"));
    }

    [AllureEpic("base")]
    private class Base;
    [AllureEpic("derived")]
    private class Derived : Base;
}
