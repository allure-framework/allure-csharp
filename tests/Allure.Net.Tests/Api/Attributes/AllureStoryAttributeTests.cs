using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureStoryAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureStoryAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task StoryCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureStoryAttribute("story value");

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("story");
        await Assert.That(attr.Value).IsEqualTo("story value");
        await AttributeAssertions.AssertLabels(tr, ("story", "story value"));
    }

    [AllureStory("base")]
    private class Base;
    [AllureStory("derived")]
    private class Derived : Base;
}
