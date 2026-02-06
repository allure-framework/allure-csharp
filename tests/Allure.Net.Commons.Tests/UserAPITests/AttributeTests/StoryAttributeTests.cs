using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class StoryAttributeTests
{
    [AllureStory("foo")]
    class TargetBase { }

    [AllureStory("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureStoryAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void StoryCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureStoryAttribute("foo");

        attr.Apply(tr);

        Assert.That(attr.Name, Is.EqualTo("story"));
        Assert.That(attr.Value, Is.EqualTo("foo"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "story", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}
