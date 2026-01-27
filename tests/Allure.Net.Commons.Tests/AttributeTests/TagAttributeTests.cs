using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class TagAttributeTests
{
    [Test]
    public void ItAddsSingleTagToTest()
    {
        TestResult tr = new();

        new AllureTagAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "tag", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void ItAddsMultipleTagsToTest()
    {
        TestResult tr = new();

        new AllureTagAttribute("foo", "bar", "baz").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "tag", value = "foo" },
                new Label { name = "tag", value = "bar" },
                new Label { name = "tag", value = "baz" },
            ]).UsingPropertiesComparer()
        );
    }
}