using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureFeatureAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureFeatureAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task FeatureCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureFeatureAttribute("feature value");

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("feature");
        await Assert.That(attr.Value).IsEqualTo("feature value");
        await AttributeAssertions.AssertLabels(tr, ("feature", "feature value"));
    }

    [AllureFeature("base")]
    private class Base;
    [AllureFeature("derived")]
    private class Derived : Base;
}
