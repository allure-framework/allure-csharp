using System.IO;
using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Attachments.AttachmentApi
{
    public class ContentAttachmentAttributes
    {
        [Fact]
        public async Task TestMethod()
        {
            _ = TextAttachment();
            _ = await BytesAttachmentAsync();
        }

        [AllureAttachment(
            "Attribute text",
            ContentType = "text/plain",
            Extension = ".txt"
        )]
        static string TextAttachment() => "Attribute text body";

        [AllureAttachment(
            "Async attribute bytes",
            ContentType = "application/octet-stream",
            Extension = ".bin"
        )]
        static async Task<byte[]> BytesAttachmentAsync()
        {
            await Task.Yield();
            return new byte[] { 21, 22 };
        }
    }

    public class FileAttachmentAttributes
    {
        [Fact]
        public async Task TestMethod()
        {
            var syncPath = FileAttachment();
            File.Delete(syncPath);

            var asyncFile = await FileAttachmentAsync();
            asyncFile.Delete();
        }

        [AllureAttachmentFile(
            "Attribute file",
            ContentType = "application/octet-stream"
        )]
        static string FileAttachment()
        {
            var path = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.bin");
            File.WriteAllBytes(path, new byte[] { 31, 32 });
            return path;
        }

        [AllureAttachmentFile(
            "Async attribute file",
            ContentType = "application/octet-stream"
        )]
        static async Task<FileInfo> FileAttachmentAsync()
        {
            var path = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.dat");
            await File.WriteAllBytesAsync(path, new byte[] { 33, 34 });
            return new FileInfo(path);
        }
    }
}
