using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class EpicAttributeTests
{
    [Test]
    public void EpicCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureEpicAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "epic", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}