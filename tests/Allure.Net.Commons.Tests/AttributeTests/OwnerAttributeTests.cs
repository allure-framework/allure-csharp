using System.Linq;
using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class OwnerAttributeTests
{
    [AllureOwner("John Doe")]
    class TargetBase { }

    [AllureOwner("Jane Doe")]
    class TargetDerived : TargetBase { }

    [Test]
    public void OwnerOfTheMostDerivedClassQueried()
    {
        TestResult tr = new();
        var attrs = typeof(TargetDerived).GetCustomAttributes<AllureOwnerAttribute>();

        Assert.That(attrs, Has.Exactly(1).Items);

        attrs.Single().Apply(tr);
        var owners = tr.labels.FindAll(static (l) => l.name == "owner");

        Assert.That(
            owners,
            Is.EqualTo([new Label { name = "owner", value = "Jane Doe" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void OwnerCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureOwnerAttribute("John Doe");

        attr.Apply(tr);

        Assert.That(attr.Name, Is.EqualTo("owner"));
        Assert.That(attr.Value, Is.EqualTo("John Doe"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "owner", value = "John Doe" }])
                .UsingPropertiesComparer()
        );
    }
}