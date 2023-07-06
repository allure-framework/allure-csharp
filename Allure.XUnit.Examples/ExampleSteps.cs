using System;
using System.Threading.Tasks;
using Allure.Xunit;
using Allure.Xunit.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Allure.XUnit.Examples;

[Obsolete("See ExampleStepAttributes")]
[AllureSuite("ExampleSteps (Obsolete)")]
public class ExampleSteps : IAsyncLifetime
{
    ITestOutputHelper output;

    public ExampleSteps(ITestOutputHelper output)
    {
        this.output = output;
    }

    public Task InitializeAsync()
    {
        using (new AllureBefore("Initialization"))
        {
            using (new AllureStep("Nested"))
            {
                return Task.CompletedTask;
            }
        }
    }

    public Task DisposeAsync()
    {
        using var _ = new AllureAfter("Cleanup");
        return Task.CompletedTask;
    }

    [Fact(Skip = "ExampleSteps is obsolete")]
    public async Task TestParameters()
    {
        WriteHello(42, 4242, "secret");
        await AddAttachment();
    }

    [Fact(Skip = "ExampleSteps is obsolete")]
    public void TestFail()
    {
        using (new AllureStep("Test Fail"))
        {
            using (new AllureStep("Nested"))
            {
                throw new Exception();
            }
        }
    }

    private void WriteHello(int parameter, int renameMe, string password)
    {
        using (new AllureStep("Write Hello").SetParameter(parameter).SetParameter("value", renameMe))
        {
            this.output.WriteLine("Hello from Step");
        }
    }

    private static async Task AddAttachment()
    {
        await AllureAttachments.Text("large json", "{}");
    }
}