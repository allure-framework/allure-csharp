using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureSuiteHierarchyAttributeTests
{
    [Test]
    public async Task CantBeInherited()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureSuiteHierarchyAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task SingleSuiteCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureSuiteHierarchyAttribute("suite value");

        attr.Apply(tr);

        await Assert.That(attr.ParentSuite).IsNull();
        await Assert.That(attr.Suite).IsEqualTo("suite value");
        await Assert.That(attr.SubSuite).IsNull();
        await AttributeAssertions.AssertLabels(tr, ("suite", "suite value"));
    }

    [Test]
    public async Task TwoLevelSuiteHierarchyCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureSuiteHierarchyAttribute("parent value", "suite value");

        attr.Apply(tr);

        await Assert.That(attr.ParentSuite).IsEqualTo("parent value");
        await Assert.That(attr.Suite).IsEqualTo("suite value");
        await Assert.That(attr.SubSuite).IsNull();
        await AttributeAssertions.AssertLabels(
            tr,
            ("parentSuite", "parent value"),
            ("suite", "suite value")
        );
    }

    [Test]
    public async Task ThreeLevelSuiteHierarchyCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureSuiteHierarchyAttribute(
            "parent value",
            "suite value",
            "sub value"
        );

        attr.Apply(tr);

        await Assert.That(attr.ParentSuite).IsEqualTo("parent value");
        await Assert.That(attr.Suite).IsEqualTo("suite value");
        await Assert.That(attr.SubSuite).IsEqualTo("sub value");
        await AttributeAssertions.AssertLabels(
            tr,
            ("parentSuite", "parent value"),
            ("suite", "suite value"),
            ("subSuite", "sub value")
        );
    }

    [AllureSuiteHierarchy("base")]
    private class Base;
    [AllureSuiteHierarchy("derived")]
    private class Derived : Base;
}
