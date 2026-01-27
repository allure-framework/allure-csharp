using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class SubSuiteAttributeTests
{
    [Test]
    public void SubSuiteCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureSubSuiteAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "subSuite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}