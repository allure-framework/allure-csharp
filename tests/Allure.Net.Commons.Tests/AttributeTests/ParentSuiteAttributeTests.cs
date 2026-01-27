using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class ParentSuiteAttributeTests
{
    [Test]
    public void ParentSuiteCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureParentSuiteAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "parentSuite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}