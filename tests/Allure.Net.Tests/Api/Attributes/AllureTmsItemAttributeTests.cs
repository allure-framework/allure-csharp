using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureTmsItemAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureTmsItemAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task TmsItemCanBeAddedToTestWithOptionalTitle()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureTmsItemAttribute("TMS-18") { Title = "Test title" };

        attr.Apply(tr);

        await Assert.That(attr.IdOrUrl).IsEqualTo("TMS-18");
        await AttributeAssertions.AssertLink(tr, "TMS-18", "Test title", "tms");
    }

    [Test]
    public async Task TmsItemWithoutTitleCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureTmsItemAttribute("TMS-18");

        attr.Apply(tr);

        await Assert.That(attr.IdOrUrl).IsEqualTo("TMS-18");
        await AttributeAssertions.AssertLink(tr, "TMS-18", null, "tms");
    }

    [Test]
    public async Task DoesNothingIfUrlIsNull()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureTmsItemAttribute(null!).Apply(tr);

        await Assert.That(tr.Links).IsEmpty();
    }

    [AllureTmsItem("BASE-TMS")]
    private class Base;
    [AllureTmsItem("DERIVED-TMS")]
    private class Derived : Base;
}
