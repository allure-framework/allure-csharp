using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureNameAttributeTests
{
    [Test]
    public async Task CantBeInherited()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureNameAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task ItSetsTestName()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureNameAttribute("Display name");

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("Display name");
        await Assert.That(tr.Name).IsEqualTo("Display name");
    }

    [Test]
    public async Task DoesNothingIfNameIsNull()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureNameAttribute(null!).Apply(tr);

        await Assert.That(tr.Name).IsEqualTo("test");
    }

    [AllureName("base")]
    private class Base;
    [AllureName("derived")]
    private class Derived : Base;
}
