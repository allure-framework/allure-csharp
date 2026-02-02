using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class AllureIdAttributeTests
{
    [Test]
    public void AllureIdCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureIdAttribute(1001);

        attr.Apply(tr);

        Assert.That(attr.Name, Is.EqualTo("ALLURE_ID"));
        Assert.That(attr.Value, Is.EqualTo("1001"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "ALLURE_ID", value = "1001" }])
                .UsingPropertiesComparer()
        );
    }
}