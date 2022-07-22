using System;
using System.Threading.Tasks;
using Allure.Xunit.Attributes;
using Allure.XUnit.Attributes.Steps;
using Xunit;

namespace Allure.XUnit.Examples;

[AllureSuite("StepAttributes")]
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

    [AllureXunit(DisplayName = "This test should be red")]
    public void TestFailed()
    {
        SomeStep();
        FailingStep();
        SomeStep();
    }
    
    [AllureXunit(DisplayName = "This test should be yellow")]
    public void TestBroken()
    {
        SomeStep();
        ExceptionStep();
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

    [AllureStep("Check that everytime you call this step it will fail")]
    private void FailingStep()
    {
        Assert.True(false);
    }
    
    [AllureStep("Check that everytime you call this step it will throw an exception")]
    private void ExceptionStep()
    {
        throw new Exception("Oh my! This is exception!");
    }

    [AllureAfter("Cleanup by simple Dispose method")]
    public void Dispose()
    {
        NestedStepReturningString("Cleanup step");
        AddAttachment();
    }
}