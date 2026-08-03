using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureBddHierarchyAttributeTests
{
    [Test]
    public async Task CantBeInherited()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureBddHierarchyAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task SingleFeatureCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureBddHierarchyAttribute("feature value");

        attr.Apply(tr);

        await Assert.That(attr.Epic).IsNull();
        await Assert.That(attr.Feature).IsEqualTo("feature value");
        await Assert.That(attr.Story).IsNull();
        await AttributeAssertions.AssertLabels(tr, ("feature", "feature value"));
    }

    [Test]
    public async Task EpicAndFeatureCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureBddHierarchyAttribute("epic value", "feature value");

        attr.Apply(tr);

        await Assert.That(attr.Epic).IsEqualTo("epic value");
        await Assert.That(attr.Feature).IsEqualTo("feature value");
        await Assert.That(attr.Story).IsNull();
        await AttributeAssertions.AssertLabels(
            tr,
            ("epic", "epic value"),
            ("feature", "feature value")
        );
    }

    [Test]
    public async Task EpicFeatureAndStoryCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureBddHierarchyAttribute(
            "epic value",
            "feature value",
            "story value"
        );

        attr.Apply(tr);

        await Assert.That(attr.Epic).IsEqualTo("epic value");
        await Assert.That(attr.Feature).IsEqualTo("feature value");
        await Assert.That(attr.Story).IsEqualTo("story value");
        await AttributeAssertions.AssertLabels(
            tr,
            ("epic", "epic value"),
            ("feature", "feature value"),
            ("story", "story value")
        );
    }

    [AllureBddHierarchy("base")]
    private class Base;
    [AllureBddHierarchy("derived")]
    private class Derived : Base;
}
