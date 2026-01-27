using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class LabelAttributeTests
{
    [Test]
    public void ItAddsLabelToTest()
    {
        TestResult tr = new();

        new AllureLabelAttribute("foo", "bar").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "foo", value = "bar" }])
                .UsingPropertiesComparer()
        );
    }
}