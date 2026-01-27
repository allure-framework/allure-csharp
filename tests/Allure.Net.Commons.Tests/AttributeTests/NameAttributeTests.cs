using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class NameAttributeTests
{
    [Test]
    public void ItSetsTestName()
    {
        TestResult tr = new();

        new AllureNameAttribute("foo").Apply(tr);

        Assert.That(tr.name, Is.EqualTo("foo"));
    }
}