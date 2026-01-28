using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

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

        new AllureStoryAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "story", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}
