using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Allure.Net.Commons;
using HeyRed.Mime;

namespace Allure.XUnit
{
    /// <summary>
    /// Decorates AllureLifecycle.Instance.AddAttachment method. Used to save attachments for reports.
    /// </summary>
    /// <para>Use <see cref="Attachments"/> instead.</para>
    /// <para>After 2.9.1 there is no reason to use methods returning async Task.</para>

    [Obsolete("Use Attachments class instead. All async operations will be removed in future release 2.10")]
    public class AllureAttachments
    {
        [Obsolete]
        public static async Task Text(string name, string content) =>
            await Bytes(name, Encoding.UTF8.GetBytes(content), ".txt");

        [Obsolete]
        public static async Task Bytes(string name, byte[] content, string extension) =>
            await AddAttachment(name, MimeTypesMap.GetMimeType(extension), content, extension);

        [Obsolete]
        public static Task File(string fileName)
        {
            return File(fileName, fileName);
        }

        [Obsolete]
        public static async Task File(string attachmentName, string fileName)
        {
            var content = await System.IO.File.ReadAllBytesAsync(fileName);
            var extension = Path.GetExtension(fileName);
            await Bytes(attachmentName, content, extension);
        }

        [Obsolete]
        public static Task AddAttachment(string name, string type, byte[] content, string fileExtension)
        {
            AllureApi.AddAttachment(name, type, content, fileExtension);
            return Task.CompletedTask;
        }
    }
}
