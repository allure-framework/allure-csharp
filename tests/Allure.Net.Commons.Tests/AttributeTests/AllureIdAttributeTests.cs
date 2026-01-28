using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class AllureIdAttributeTests
{
    [Test]
    public void AllureIdCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureIdAttribute(1001).Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "ALLURE_ID", value = "1001" }])
                .UsingPropertiesComparer()
        );
    }
}