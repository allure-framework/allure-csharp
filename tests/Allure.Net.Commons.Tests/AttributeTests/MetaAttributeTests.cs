using System;
using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;


class MetaAttributeTests
{
    [AllureTag("foo")]
    [AllureSuite("bar")]
    [AllureIssue("foo", Title = "bar")]
    [AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
    class FooAttribute : AllureMetaAttribute { }

    [Test]
    public void AttributesOfTheMetaAttributeAreApplied()
    {
        TestResult tr = new();

        new FooAttribute().Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "tag", value = "foo" },
                new Label { name = "suite", value = "bar" },
            ]).UsingPropertiesComparer()
        );
        Assert.That(
            tr.links,
            Is.EquivalentTo([new Link { url = "foo", name = "bar", type = "issue" }])
                .UsingPropertiesComparer()
        );
    }
}
