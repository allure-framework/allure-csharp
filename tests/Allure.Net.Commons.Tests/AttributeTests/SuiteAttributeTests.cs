using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class SuiteAttributeTests
{
    [Test]
    public void SuiteCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureSuiteAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "suite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}