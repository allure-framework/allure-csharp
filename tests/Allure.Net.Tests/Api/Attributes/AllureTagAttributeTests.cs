using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureTagAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureTagAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task ItAddsSingleTagToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureTagAttribute("first");

        attr.Apply(tr);

        await Assert.That(attr.Tags).IsEquivalentTo(["first"]);
        await AttributeAssertions.AssertLabels(tr, ("tag", "first"));
    }

    [Test]
    public async Task ItAddsMultipleTagsToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureTagAttribute("first", "second", "third");

        attr.Apply(tr);

        await Assert.That(attr.Tags).IsEquivalentTo(["first", "second", "third"]);
        await AttributeAssertions.AssertLabels(
            tr,
            ("tag", "first"),
            ("tag", "second"),
            ("tag", "third")
        );
    }

    [Test]
    public async Task NullTagsAreIgnored()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureTagAttribute(null!, null!, null!);

        attr.Apply(tr);

        await Assert.That(attr.Tags.Length).IsEqualTo(3);
        await Assert.That(tr.Labels).IsEmpty();
    }

    [Test]
    public async Task EmptyTagsAreIgnored()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureTagAttribute("", "", "");

        attr.Apply(tr);

        await Assert.That(attr.Tags).IsEquivalentTo(["", "", ""]);
        await Assert.That(tr.Labels).IsEmpty();
    }

    [AllureTag("base")]
    private class Base;
    [AllureTag("derived")]
    private class Derived : Base;
}
