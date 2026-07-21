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

    static async Task AssertUsage<T>(AttributeTargets targets, bool inherited)
        where T : Attribute
    {
        var usage = typeof(T).GetCustomAttribute<AttributeUsageAttribute>()!;
        await Assert.That(usage.ValidOn).IsEqualTo(targets);
        await Assert.That(usage.AllowMultiple).IsFalse();
        await Assert.That(usage.Inherited).IsEqualTo(inherited);
    }
}
