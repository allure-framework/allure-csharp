using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureLabelAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureLabelAttribute>();

        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task ItAddsLabelToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureLabelAttribute("custom", "value");

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("custom");
        await Assert.That(attr.Value).IsEqualTo("value");
        await AttributeAssertions.AssertLabels(tr, ("custom", "value"));
    }

    [Test]
    public async Task DoesNothingIfNameIsNull()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureLabelAttribute(null!, "value").Apply(tr);

        await Assert.That(tr.Labels).IsEmpty();
    }

    [Test]
    public async Task DoesNothingIfNameIsEmpty()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureLabelAttribute("", "value").Apply(tr);

        await Assert.That(tr.Labels).IsEmpty();
    }

    [Test]
    public async Task DoesNothingIfValueIsNull()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureLabelAttribute("custom", null!).Apply(tr);

        await Assert.That(tr.Labels).IsEmpty();
    }

    [AllureLabel("base", "value")]
    private class Base;

    [AllureLabel("derived", "value")]
    private class Derived : Base;
}
