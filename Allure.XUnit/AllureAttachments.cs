using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Xunit;
using HeyRed.Mime;

namespace Allure.XUnit
{
    public class AllureAttachments
    {
        public static async Task Text(string name, string content) =>
            await Bytes(name, Encoding.UTF8.GetBytes(content), ".txt");

        public static async Task Bytes(string name, byte[] content, string extension) =>
            await AddAttachment(name, MimeTypesMap.GetMimeType(extension), content, extension);

        public static Task File(string fileName)
        {
            return File(fileName, fileName);
        }

        public static async Task File(string attachmentName, string fileName)
        {
            var content = await System.IO.File.ReadAllBytesAsync(fileName);
            var extension = Path.GetExtension(fileName);
            await Bytes(attachmentName, content, extension);
        }

        public static async Task AddAttachment(string name, string type, byte[] content, string fileExtension)
        {
            var source = $"{Guid.NewGuid():N}{AllureConstants.ATTACHMENT_FILE_SUFFIX}{fileExtension}";
            await System.IO.File.WriteAllBytesAsync(Path.Combine(AllureLifecycle.Instance.ResultsDirectory, source),
                content);
            var attachments = Steps.Current.attachments ??= new();
            attachments.Add(new()
            {
                name = name,
                type = type,
                source = source
            });
        }
    }
}
