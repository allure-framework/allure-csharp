using System;
using System.Threading.Tasks;
using Allure.Xunit.Attributes;
using Allure.XUnit.Attributes.Steps;
using Xunit;

namespace Allure.XUnit.Examples;

public class ExampleStepAttributes : IDisposable
{
    [AllureBefore("Initialization in constructor")]
    public ExampleStepAttributes()
    {
        AddAttachment();
        NestedStep(1);
        NestedStepReturningString("Second");
    }

    [AllureXunit]
    public void Test()
    {
        WriteHelloStep(42, 4242, "secret");
        SomeStep();
        AddAttachment();
        SomeStep();
    }

    [AllureXunit]
    public void TestSecond()
    {
        SomeStep();
    }

    [AllureStep("Write Hello")]
    private void WriteHelloStep(int parameter, [Name("value")] int renameMe, [Skip] string password)
    {
        AddAttachment();
        NestedStepReturningString("Write hello nested step");
    }

    [AllureStep("Add Attachment")]
    private void AddAttachment()
    {
        Attachments.Text("Json file", "{\"id\":42,\"name\":\"Allure.XUnit\"}");
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

    [AllureAfter("Cleanup by simple Dispose method")]
    public void Dispose()
    {
        NestedStepReturningString("Cleanup step");
        AddAttachment();
    }
}