using System.Linq;
using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class SeverityAttributeTests
{
    [AllureSeverity(SeverityLevel.normal)]
    class TargetBase { }

    [AllureSeverity(SeverityLevel.blocker)]
    class TargetDerived : TargetBase { }

    [Test]
    public void SeverityOfTheMostDerivedClassQueried()
    {
        TestResult tr = new();
        var attrs = typeof(TargetDerived).GetCustomAttributes<AllureSeverityAttribute>();

        Assert.That(attrs, Has.Exactly(1).Items);

        attrs.Single().Apply(tr);
        var severities = tr.labels.FindAll(static (l) => l.name == "severity");

        Assert.That(
            severities,
            Is.EqualTo([new Label { name = "severity", value = "blocker" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void SeverityCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureSeverityAttribute(SeverityLevel.critical);

        attr.Apply(tr);

        Assert.That(attr.Name, Is.EqualTo("severity"));
        Assert.That(attr.Value, Is.EqualTo("critical"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "severity", value = "critical" }])
                .UsingPropertiesComparer()
        );
    }
}