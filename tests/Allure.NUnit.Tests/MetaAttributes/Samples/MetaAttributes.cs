using System;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Allure.NUnit;
using NUnit.Framework;

namespace Allure.NUnit.Tests.MetaAttributes.Samples.MetaAttributes
{
    [AllureEpic("Foo")]
    [AllureOwner("John Doe")]
    [AttributeUsage(AttributeTargets.Interface)]
    public class EpicOwnerAttribute : AllureMetaAttribute { }

    [AllureFeature("Bar")]
    [AllureTag("foo", "bar")]
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureTagsAttribute : AllureMetaAttribute { }

    [AllureStory("Baz")]
    [AllureLink("https://foo.bar/")]
    [AttributeUsage(AttributeTargets.Class)]
    public class StoryLinkAttribute : AllureMetaAttribute { }

    [AllureSeverity(SeverityLevel.critical)]
    [AllureSuite("Qux")]
    [AttributeUsage(AttributeTargets.Method)]
    public class SeveritySuiteAttribute : AllureMetaAttribute { }

    [EpicOwner]
    public interface IMetadata { }

    [FeatureTags]
    public class BaseClass { }

    [AllureNUnit]
    [StoryLink]
    public class TestsClass : BaseClass, IMetadata
    {
        [Test]
        [SeveritySuite]
        public void TestMethod() { }
    }
}
