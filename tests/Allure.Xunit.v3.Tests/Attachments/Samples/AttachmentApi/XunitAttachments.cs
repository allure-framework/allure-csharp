using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Attachments.AttachmentApi;

public class XunitAttachments(ITestOutputHelper helper)
{
    [Fact]
    public void TestMethod()
    {
        TestContext.Current.AddAttachment("xUnit text", "xUnit text body");
        TestContext.Current.AddAttachment(
            "xUnit binary",
            new byte[] { 41, 42, 43 },
            "application/octet-stream"
        );
        helper.Write("stdout content");
    }
}
