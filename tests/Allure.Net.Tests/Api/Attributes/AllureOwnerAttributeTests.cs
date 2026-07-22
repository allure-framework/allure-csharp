using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureOwnerAttributeTests
{
    [Test]
    public async Task OwnerOfMostDerivedClassIsQueried()
    {
        var attr = typeof(Derived).GetCustomAttributes<AllureOwnerAttribute>().Single();
        await Assert.That(attr.Value).IsEqualTo("derived owner");
    }

    [Test]
    public async Task OwnerCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureOwnerAttribute("owner value");

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("owner");
        await Assert.That(attr.Value).IsEqualTo("owner value");
        await AttributeAssertions.AssertLabels(tr, ("owner", "owner value"));
    }

    [AllureOwner("base owner")]
    private class Base;
    [AllureOwner("derived owner")]
    private class Derived : Base;
}
