using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class DescriptionAttributeTests
{
    [Test]
    public void SetsTestDescription()
    {
        TestResult tr = new();

        new AllureDescriptionAttribute("foo").Apply(tr);

        Assert.That(tr.description, Is.EqualTo("foo"));
    }
}