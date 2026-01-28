using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Sdk;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.FunctionTests.ModelFunctionTests;

class AttributeApplicationTests
{
    [Test]
    public void DirectMethodAttributesAreApplied()
    {
        TestResult tr = new();
        var method = typeof(MethodsWithAttrs).GetMethod(nameof(MethodsWithAttrs.Foo));

        AllureMetadataAttribute.ApplyMethodAttributes(tr, method);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "Foo epic" },
                new Label { name = "feature", value = "Foo feature" },
                new Label { name = "story", value = "Foo story" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void AbstractBaseAttributesAreApplies()
    {
        TestResult tr = new();
        var method = typeof(MethodsWithAttrs).GetMethod(nameof(MethodsWithAttrs.Bar));

        AllureMetadataAttribute.ApplyMethodAttributes(tr, method);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "Bar epic" },
                new Label { name = "feature", value = "Bar feature" },
                new Label { name = "story", value = "Bar story" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void VirtualBaseAttributesAreApplies()
    {
        TestResult tr = new();
        var method = typeof(MethodsWithAttrs).GetMethod(nameof(MethodsWithAttrs.Baz));

        AllureMetadataAttribute.ApplyMethodAttributes(tr, method);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "Baz epic" },
                new Label { name = "feature", value = "Baz feature" },
                new Label { name = "story", value = "Baz story" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void AppliesBaseBeforeOverride()
    {
        TestResult tr = new();
        var method = typeof(AttributeApplicationOrderChild)
            .GetMethod(nameof(AttributeApplicationOrderChild.TargetMethod));

        AllureMetadataAttribute.ApplyMethodAttributes(tr, method);

        Assert.That(tr.description, Is.EqualTo("baz\n\nqut"));
    }

    [Test]
    public void DirectTypeAttributesAreApplied()
    {
        TestResult tr = new();

        AllureMetadataAttribute.ApplyTypeAttributes(tr, typeof(ClassWithAttrs));

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "Base epic" },
                new Label { name = "feature", value = "Base feature" },
                new Label { name = "story", value = "Base story" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void AttributesFromBaseClassAreApplied()
    {
        TestResult tr = new();

        AllureMetadataAttribute.ApplyTypeAttributes(tr, typeof(InheritedFromClassAttributes));

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "Base epic" },
                new Label { name = "feature", value = "Base feature" },
                new Label { name = "story", value = "Base story" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void AttributesFromInterfaceAreApplied()
    {
        TestResult tr = new();

        AllureMetadataAttribute.ApplyTypeAttributes(tr, typeof(InheritedFromInterfaceAttributes));

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "Interface epic" },
                new Label { name = "feature", value = "Interface feature" },
                new Label { name = "story", value = "Interface story" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void AttributesFromDifferentSourcesAreCombined()
    {
        TestResult tr = new();

        AllureMetadataAttribute.ApplyTypeAttributes(tr, typeof(MultiSourceAttributes));

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "Base epic" },
                new Label { name = "feature", value = "Base feature" },
                new Label { name = "story", value = "Base story" },
                new Label { name = "epic", value = "Interface epic" },
                new Label { name = "feature", value = "Interface feature" },
                new Label { name = "story", value = "Interface story" },
                new Label { name = "epic", value = "Direct epic" },
                new Label { name = "feature", value = "Direct feature" },
                new Label { name = "story", value = "Direct story" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void CheckTypeAttributeApplicationOrder()
    {
        TestResult tr = new();

        AllureMetadataAttribute.ApplyTypeAttributes(tr, typeof(AttributeApplicationOrderChild));

        Assert.That(tr.description, Is.EqualTo("foo\n\nbar\n\nqux"));
    }

    [Test]
    public void ApplyAllAttributesToMethodAndItsTypeAtOnce()
    {
        TestResult tr = new();
        var method = typeof(ApplyAllInherited)
            .GetMethod(nameof(ApplyAllInherited.TargetMethod));

        AllureMetadataAttribute.ApplyAllAttributes(tr, method);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "Interface epic" },
                new Label { name = "feature", value = "Interface feature" },
                new Label { name = "story", value = "Interface story" },
                new Label { name = "epic", value = "Base epic" },
                new Label { name = "feature", value = "Base feature" },
                new Label { name = "story", value = "Base story" },
                new Label { name = "epic", value = "Derived epic" },
                new Label { name = "feature", value = "Derived feature" },
                new Label { name = "story", value = "Derived story" },
                new Label { name = "epic", value = "Base method epic" },
                new Label { name = "feature", value = "Base method feature" },
                new Label { name = "story", value = "Base method story" },
                new Label { name = "epic", value = "Derived method epic" },
                new Label { name = "feature", value = "Derived method feature" },
                new Label { name = "story", value = "Derived method story" },
            ]).UsingPropertiesComparer()
        );
    }

    #region Types to check attribute application to methods

    abstract class MethodsWithAttrsBase
    {
        [AllureEpic("Bar epic")]
        [AllureFeature("Bar feature")]
        [AllureStory("Bar story")]
        public abstract void Bar();

        [AllureEpic("Baz epic")]
        [AllureFeature("Baz feature")]
        [AllureStory("Baz story")]
        public virtual void Baz() { }
    }

    class MethodsWithAttrs : MethodsWithAttrsBase
    {
        [AllureEpic("Foo epic")]
        [AllureFeature("Foo feature")]
        [AllureStory("Foo story")]
        public static void Foo() { }

        public override void Bar() { }

        public override void Baz() { }
    }

    #endregion

    #region Types to check attribute application to types

    [AllureEpic("Base epic")]
    [AllureFeature("Base feature")]
    [AllureStory("Base story")]
    class ClassWithAttrs { }

    class InheritedFromClassAttributes : ClassWithAttrs { }

    [AllureEpic("Interface epic")]
    [AllureFeature("Interface feature")]
    [AllureStory("Interface story")]
    interface IInterfaceWithAttributes { }

    class InheritedFromInterfaceAttributes : IInterfaceWithAttributes { }

    [AllureEpic("Direct epic")]
    [AllureFeature("Direct feature")]
    [AllureStory("Direct story")]
    class MultiSourceAttributes : ClassWithAttrs, IInterfaceWithAttributes { }

    #endregion

    #region Types to check attribute application to method and type

    [AllureEpic("Interface epic")]
    [AllureFeature("Interface feature")]
    [AllureStory("Interface story")]
    interface IApplyAllInterface { }

    [AllureEpic("Base epic")]
    [AllureFeature("Base feature")]
    [AllureStory("Base story")]
    class ApplyAllBase
    {
        [AllureEpic("Base method epic")]
        [AllureFeature("Base method feature")]
        [AllureStory("Base method story")]
        public virtual void TargetMethod() { }
    }

    [AllureEpic("Derived epic")]
    [AllureFeature("Derived feature")]
    [AllureStory("Derived story")]
    class ApplyAllInherited : ApplyAllBase, IApplyAllInterface
    {
        [AllureEpic("Derived method epic")]
        [AllureFeature("Derived method feature")]
        [AllureStory("Derived method story")]
        public override void TargetMethod() { }
    }

    #endregion

    #region Types to check the attribute application order

    [AllureDescription("foo", Append = true)]
    interface IAttributeApplicationOrder { }

    [AllureDescription("bar", Append = true)]
    class AttributeApplicationOrderBase
    {
        [AllureDescription("baz", Append = true)]
        public virtual void TargetMethod() { }
    }

    [AllureDescription("qux", Append = true)]
    class AttributeApplicationOrderChild : AttributeApplicationOrderBase, IAttributeApplicationOrder
    {
        [AllureDescription("qut", Append = true)]
        public override void TargetMethod() { }
    }

    #endregion
}