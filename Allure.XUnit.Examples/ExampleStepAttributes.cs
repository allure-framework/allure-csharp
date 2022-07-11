using System.Threading.Tasks;
using Allure.Xunit.Attributes;
using Allure.XUnit.Attributes.Steps;
using Xunit;

namespace Allure.XUnit.Examples;

public class ExampleStepAttributes : IAsyncLifetime
{
    [AllureBefore("Initialization")]
    public Task InitializeAsync()
    {
        NestedStep(1);
        NestedStepReturningString("Second");
        return Task.CompletedTask;
    }

    [AllureXunit]
    public async Task Test()
    {
        WriteHelloStep(42, 4242, "secret");
        SomeStep();
        await AddAttachmentAsync();
        SomeStep();
    }

    [AllureStep("Write Hello")]
    private void WriteHelloStep(int parameter, [Name("value")] int renameMe, [Skip] string password)
    {
        AddAttachment();
        NestedStepReturningString("Write hello nested step");
    }

    [AllureStep("Add Attachment asynchronously")]
    private async Task AddAttachmentAsync()
    {
        await AllureAttachments.Text("large json", "{}");
    }

    [AllureStep("Add Attachment")]
    private void AddAttachment()
    {
        AllureAttachments.Text("large json", "{}").GetAwaiter().GetResult();
    }

    [AllureStep("Another nested step with \"{input}\"")]
    private string NestedStepReturningString([Name("Input text parameter")] string input)
    {
        Assert.True(true);
        return input;
    }

    [AllureStep("Nested step {i}")]
    private void NestedStep([Name("number")] int i, [Skip] bool skippedBoolean = true)
    {
        Assert.Equal(i, i);
    }

    [AllureStep("Some step")]
    private void SomeStep()
    {
        Assert.True(true);
    }

    [AllureAfter("Cleanup")]
    public Task DisposeAsync()
    {
        NestedStepReturningString("Cleanup nested 1");
        NestedStep(2);
        
        AllureMessageBus.TestOutputHelper.Value.WriteLine("Hello from dispose");
        return Task.CompletedTask;
    }
}