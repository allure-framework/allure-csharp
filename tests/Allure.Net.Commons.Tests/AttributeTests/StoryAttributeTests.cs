using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class StoryAttributeTests
{
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
