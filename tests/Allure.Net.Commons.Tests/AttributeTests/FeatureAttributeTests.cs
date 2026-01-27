using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class FeatureAttributeTests
{
    [Test]
    public void FeatureCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureFeatureAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "feature", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}
