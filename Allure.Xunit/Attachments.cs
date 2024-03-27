using System;
using System.ComponentModel;
using System.Text;
using Allure.Net.Commons;
using Allure.Xunit;
using HeyRed.Mime;

namespace Allure.Xunit
{
    public static class Attachments
    {
        public static void Text(string name, string content) => Bytes(name, Encoding.UTF8.GetBytes(content), ".txt");
        public static void Bytes(string name, byte[] content, string extension = "") =>
            AllureApi.AddAttachment(name, MimeTypesMap.GetMimeType(extension), content, extension);
        public static void File(string name, string path) =>
            AllureApi.AddAttachment(path, name);
        public static void File(string fileName) => File(fileName, fileName);
    }
}

namespace Allure.XUnit
{
    [Obsolete(AllureXunitHelper.NS_OBSOLETE_MSG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class Attachments
    {
        public static void Text(string name, string content) =>
            Xunit.Attachments.Text(name, content);
        public static void Bytes(string name, byte[] content, string extension = "") =>
            Xunit.Attachments.Bytes(name, content, extension);
        public static void File(string name, string path) =>
            Xunit.Attachments.File(name, path);
        public static void File(string fileName) =>
            Xunit.Attachments.File(fileName);
    }
}