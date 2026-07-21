using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Aspects;

public class AspectWeavingTests
{
    [Test]
    public async Task StepAttributeInjectsStepAspect()
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint());

        var result = WovenStep();

        await Assert.That(result).IsEqualTo(9);
        await Assert.That(operations.Sync.SingleCall.Method.Name).IsEqualTo("Step");
        await Assert.That(operations.Sync.SingleCall.Arguments[0]).IsEqualTo("woven step");
    }

    [Test]
    public async Task AsyncAttachmentAttributeInjectsAttachmentAspect()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        byte[]? content = null;
        operations.Handler = (_, arguments) =>
        {
            using var copy = new MemoryStream();
            ((Stream)arguments[1]!).CopyTo(copy);
            content = copy.ToArray();
            return null;
        };
        using var scope = FacadeTestEnvironment.Use(
            current: new TestApiEndpoint(sync: operations.Instance)
        );

        var result = await WovenAsyncAttachment();

        await Assert.That(result).IsEqualTo("async content");
        await Assert.That(operations.SingleCall.Method.Name).IsEqualTo("AddAttachment");
        await Assert.That(content).IsEquivalentTo(System.Text.Encoding.UTF8.GetBytes(result));
    }

    [AllureStep("woven step")]
    static int WovenStep() => 9;

    [AllureAttachment("woven attachment")]
    static async Task<string> WovenAsyncAttachment()
    {
        await Task.Yield();
        return "async content";
    }
}
