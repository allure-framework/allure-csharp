using System;
using System.Threading.Tasks;
using Allure.XUnit;
using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Examples;

public class ExampleSteps : IAsyncLifetime
{
    public Task InitializeAsync()
    {
        using (new AllureBefore("Initialization"))
        {
            using (new AllureBefore("Nested"))
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

    [AllureXunit]
    public async Task TestParameters()
    {
        WriteHello(42, 4242, "secret");
        await AddAttachment();
    }

    [AllureXunit]
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

    private static void WriteHello(int parameter, int renameMe, string password)
    {
        using (new AllureStep("Write Hello").SetParameter(parameter).SetParameter("value", renameMe))
        {
            AllureMessageBus.TestOutputHelper.Value.WriteLine("Hello from Step");
        }
    }

    private static async Task AddAttachment()
    {
        await AllureAttachments.Text("large json", "{}");
    }
}