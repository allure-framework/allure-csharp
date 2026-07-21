using System.Reflection;

namespace Allure.Net.Tests.ApiContract;

public class AttributeContractTests
{
    [Test]
    public async Task OperationAttributesHaveExpectedTargets()
    {
        await AssertUsage<AllureStepAttribute>(AttributeTargets.Method, inherited: true);
        await AssertUsage<AllureBeforeAttribute>(AttributeTargets.Method | AttributeTargets.Constructor, inherited: true);
        await AssertUsage<AllureAfterAttribute>(AttributeTargets.Method, inherited: true);
    }

    [Test]
    public async Task AttachmentAttributesHaveExpectedTargets()
    {
        await AssertUsage<AllureAttachmentAttribute>(AttributeTargets.Method, inherited: true);
        await AssertUsage<AllureAttachmentFileAttribute>(AttributeTargets.Method, inherited: true);
    }

    [Test]
    public async Task ParameterAttributeHasExpectedTarget()
    {
        await AssertUsage<AllureParameterAttribute>(AttributeTargets.Parameter, inherited: true);
    }

    [Test]
    public async Task OperationAttributeConstructorsPreserveOptionalNames()
    {
        await Assert.That(new AllureStepAttribute().Name).IsNull();
        await Assert.That(new AllureStepAttribute("step").Name).IsEqualTo("step");
        await Assert.That(new AllureBeforeAttribute().Name).IsNull();
        await Assert.That(new AllureBeforeAttribute("before").Name).IsEqualTo("before");
        await Assert.That(new AllureAfterAttribute().Name).IsNull();
        await Assert.That(new AllureAfterAttribute("after").Name).IsEqualTo("after");
        await Assert.That(new AllureSetUpAttribute().Name).IsNull();
        await Assert.That(new AllureSetUpAttribute("set-up").Name).IsEqualTo("set-up");
        await Assert.That(new AllureTearDownAttribute().Name).IsNull();
        await Assert.That(new AllureTearDownAttribute("tear-down").Name).IsEqualTo("tear-down");
    }

    static async Task AssertUsage<T>(AttributeTargets targets, bool inherited)
        where T : Attribute
    {
        var usage = typeof(T).GetCustomAttribute<AttributeUsageAttribute>()!;
        await Assert.That(usage.ValidOn).IsEqualTo(targets);
        await Assert.That(usage.AllowMultiple).IsFalse();
        await Assert.That(usage.Inherited).IsEqualTo(inherited);
    }
}
