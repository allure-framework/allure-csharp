using System.Reflection;
using Allure.Abstractions;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AttributeApplicationTests
{
    [Test]
    public async Task DirectMethodAttributesAreAppliedAndReturned()
    {
        var method = Method<MethodsWithAttrs>(nameof(MethodsWithAttrs.Direct));
        TestResult applied = new() { Name = "test", Uuid = "id" };
        TestResult returned = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyMethodAttributes(method, applied);
        AllureApiAttribute.ApplyAttributes(
            AllureApiAttribute.GetMethodAttributes(method),
            returned
        );

        await AssertBddLabels(applied, "Direct");
        await AssertBddLabels(returned, "Direct");
    }

    [Test]
    public async Task AbstractBaseAttributesAreAppliedAndReturned()
    {
        var method = Method<MethodsWithAttrs>(nameof(MethodsWithAttrs.Abstract));
        TestResult applied = new() { Name = "test", Uuid = "id" };
        TestResult returned = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyMethodAttributes(method, applied);
        AllureApiAttribute.ApplyAttributes(
            AllureApiAttribute.GetMethodAttributes(method),
            returned
        );

        await AssertBddLabels(applied, "Abstract");
        await AssertBddLabels(returned, "Abstract");
    }

    [Test]
    public async Task VirtualBaseAttributesAreAppliedAndReturned()
    {
        var method = Method<MethodsWithAttrs>(nameof(MethodsWithAttrs.Virtual));
        TestResult applied = new() { Name = "test", Uuid = "id" };
        TestResult returned = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyMethodAttributes(method, applied);
        AllureApiAttribute.ApplyAttributes(
            AllureApiAttribute.GetMethodAttributes(method),
            returned
        );

        await AssertBddLabels(applied, "Virtual");
        await AssertBddLabels(returned, "Virtual");
    }

    [Test]
    public async Task BaseMethodAttributesPrecedeOverrideAttributes()
    {
        var method = Method<ApplicationOrderChild>(nameof(ApplicationOrderChild.Target));
        TestResult applied = new() { Name = "test", Uuid = "id" };
        TestResult returned = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyMethodAttributes(method, applied);
        AllureApiAttribute.ApplyAttributes(
            AllureApiAttribute.GetMethodAttributes(method),
            returned
        );

        await Assert.That(applied.Description).IsEqualTo("base method\n\nderived method");
        await Assert.That(returned.Description).IsEqualTo("base method\n\nderived method");
    }

    [Test]
    public async Task DirectAndInheritedTypeAttributesAreAppliedAndReturned()
    {
        TestResult direct = new() { Name = "test", Uuid = "id" };
        TestResult inherited = new() { Name = "test", Uuid = "id" };
        TestResult returned = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyTypeAttributes(typeof(BaseAttrs), direct);
        AllureApiAttribute.ApplyTypeAttributes(typeof(InheritedAttrs), inherited);
        AllureApiAttribute.ApplyAttributes(
            AllureApiAttribute.GetTypeAttributes(typeof(InheritedAttrs)),
            returned
        );

        await AssertBddLabels(direct, "Base");
        await AssertBddLabels(inherited, "Base");
        await AssertBddLabels(returned, "Base");
    }

    [Test]
    public async Task InterfaceAttributesAreAppliedAndReturned()
    {
        TestResult applied = new() { Name = "test", Uuid = "id" };
        TestResult returned = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyTypeAttributes(typeof(InterfaceAttrs), applied);
        AllureApiAttribute.ApplyAttributes(
            AllureApiAttribute.GetTypeAttributes(typeof(InterfaceAttrs)),
            returned
        );

        await AssertBddLabels(applied, "Interface");
        await AssertBddLabels(returned, "Interface");
    }

    [Test]
    public async Task AttributesFromDifferentSourcesAreCombined()
    {
        TestResult applied = new() { Name = "test", Uuid = "id" };
        TestResult returned = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyTypeAttributes(typeof(MultiSourceAttrs), applied);
        AllureApiAttribute.ApplyAttributes(
            AllureApiAttribute.GetTypeAttributes(typeof(MultiSourceAttrs)),
            returned
        );

        await Assert.That(applied.Labels.Count).IsEqualTo(9);
        await Assert.That(returned.Labels.Count).IsEqualTo(9);
        await Assert.That(applied.Labels.Select(label => label.Value))
            .IsEquivalentTo(ExpectedMultiSourceValues());
        await Assert.That(returned.Labels.Select(label => label.Value))
            .IsEquivalentTo(ExpectedMultiSourceValues());
    }

    [Test]
    public async Task TypeAttributesAreAppliedInInterfaceBaseDerivedOrder()
    {
        TestResult applied = new() { Name = "test", Uuid = "id" };
        TestResult returned = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyTypeAttributes(typeof(ApplicationOrderChild), applied);
        AllureApiAttribute.ApplyAttributes(
            AllureApiAttribute.GetTypeAttributes(typeof(ApplicationOrderChild)),
            returned
        );

        const string expected = "interface\n\nbase type\n\nderived type";
        await Assert.That(applied.Description).IsEqualTo(expected);
        await Assert.That(returned.Description).IsEqualTo(expected);
    }

    [Test]
    public async Task AllAttributesCombineTypeAndMethodMetadata()
    {
        var method = Method<ApplyAllDerived>(nameof(ApplyAllDerived.Target));
        TestResult applied = new() { Name = "test", Uuid = "id" };
        TestResult returned = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyAllAttributes(method, applied);
        AllureApiAttribute.ApplyAttributes(
            AllureApiAttribute.GetAllAttributes(method),
            returned
        );

        string[] expected = [
            "Interface", "Base", "Derived", "Base method", "Derived method",
        ];
        await Assert.That(applied.Labels.Select(label => label.Value))
            .IsEquivalentTo(expected);
        await Assert.That(returned.Labels.Select(label => label.Value))
            .IsEquivalentTo(expected);
    }

    [Test]
    public async Task AllureNameFromTypeIsNotApplied()
    {
        var method = Method<TypeWithName>(nameof(TypeWithName.Target));
        TestResult tr = new() { Name = "test", Uuid = "id" };

        AllureApiAttribute.ApplyTypeAttributes(typeof(TypeWithName), tr);
        AllureApiAttribute.ApplyAllAttributes(method, tr);

        await Assert.That(tr.Name).IsEqualTo("test");
    }

    static MethodInfo Method<T>(string name) => typeof(T).GetMethod(name)!;

    static async Task AssertBddLabels(TestResult tr, string prefix)
    {
        await Assert.That(tr.Labels.Select(label => $"{label.Name}:{label.Value}"))
            .IsEquivalentTo([
                $"epic:{prefix} epic",
                $"feature:{prefix} feature",
                $"story:{prefix} story",
            ]);
    }

    static string[] ExpectedMultiSourceValues() => [
        "Interface epic", "Interface feature", "Interface story",
        "Base epic", "Base feature", "Base story",
        "Derived epic", "Derived feature", "Derived story",
    ];

    private abstract class MethodsWithAttrsBase
    {
        [AllureEpic("Abstract epic")]
        [AllureFeature("Abstract feature")]
        [AllureStory("Abstract story")]
        public abstract void Abstract();

        [AllureEpic("Virtual epic")]
        [AllureFeature("Virtual feature")]
        [AllureStory("Virtual story")]
        public virtual void Virtual() { }
    }

    private sealed class MethodsWithAttrs : MethodsWithAttrsBase
    {
        [AllureEpic("Direct epic")]
        [AllureFeature("Direct feature")]
        [AllureStory("Direct story")]
        public static void Direct() { }
        public override void Abstract() { }
        public override void Virtual() { }
    }

    [AllureEpic("Base epic")]
    [AllureFeature("Base feature")]
    [AllureStory("Base story")]
    private class BaseAttrs;
    private sealed class InheritedAttrs : BaseAttrs;

    [AllureEpic("Interface epic")]
    [AllureFeature("Interface feature")]
    [AllureStory("Interface story")]
    private interface IAttrs;
    private sealed class InterfaceAttrs : IAttrs;

    [AllureEpic("Derived epic")]
    [AllureFeature("Derived feature")]
    [AllureStory("Derived story")]
    private sealed class MultiSourceAttrs : BaseAttrs, IAttrs;

    [AllureDescription("interface", Append = true)]
    private interface IApplicationOrder;

    [AllureDescription("base type", Append = true)]
    private class ApplicationOrderBase
    {
        [AllureDescription("base method", Append = true)]
        public virtual void Target() { }
    }

    [AllureDescription("derived type", Append = true)]
    private sealed class ApplicationOrderChild : ApplicationOrderBase, IApplicationOrder
    {
        [AllureDescription("derived method", Append = true)]
        public override void Target() { }
    }

    [AllureLabel("source", "Interface")]
    private interface IApplyAll;

    [AllureLabel("source", "Base")]
    private class ApplyAllBase
    {
        [AllureLabel("source", "Base method")]
        public virtual void Target() { }
    }

    [AllureLabel("source", "Derived")]
    private sealed class ApplyAllDerived : ApplyAllBase, IApplyAll
    {
        [AllureLabel("source", "Derived method")]
        public override void Target() { }
    }

    [AllureName("Type name")]
    private sealed class TypeWithName
    {
        public static void Target() { }
    }
}
