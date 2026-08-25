using System.IO;
using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Attachments.AttachmentApi
{
    public class SyncRuntimeAttachments
    {
        [Fact]
        public void TestMethod()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            AllureApi.AddAttachment(
                "Sync stream",
                stream,
                "application/octet-stream",
                ".bin"
            );
            AllureApi.AddAttachment(
                "Sync memory",
                new byte[] { 4, 5 },
                "application/octet-stream",
                ".dat"
            );
            AllureApi.AddAttachment(
                "Sync text",
                "Sync text body",
                "text/plain",
                ".txt"
            );

            var path = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.source");
            try
            {
                File.WriteAllBytes(path, new byte[] { 6, 7 });
                AllureApi.AddAttachmentFromFile(
                    path,
                    "Sync file",
                    "application/octet-stream",
                    ".raw"
                );
            }
            finally
            {
                File.Delete(path);
            }
        }
    }

    public class AsyncRuntimeAttachments
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            using var stream = new MemoryStream(new byte[] { 11, 12, 13 });
            await AllureApi.AddAttachmentAsync(
                "Async stream",
                stream,
                "application/octet-stream",
                ".bin",
                token
            );
            await AllureApi.AddAttachmentAsync(
                "Async memory",
                new byte[] { 14, 15 },
                "application/octet-stream",
                ".dat",
                token
            );
            await AllureApi.AddAttachmentAsync(
                "Async text",
                "Async text body",
                "text/plain",
                ".txt",
                token
            );

            var path = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.source");
            try
            {
                await File.WriteAllBytesAsync(path, new byte[] { 16, 17 }, token);
                await AllureApi.AddAttachmentFromFileAsync(
                    path,
                    "Async file",
                    "application/octet-stream",
                    ".raw",
                    token
                );
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
