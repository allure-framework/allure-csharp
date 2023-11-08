using System.Text;
using Allure.Net.Commons;
using HeyRed.Mime;

namespace NUnit.Allure
{
    public abstract class Attachments
    {
        public static void Text(string name, string content) => Bytes(name, Encoding.UTF8.GetBytes(content), ".txt");
        public static void Bytes(string name, byte[] content, string extension = "") =>
            AllureApi.AddAttachment(name, MimeTypesMap.GetMimeType(extension), content, extension);
        public static void File(string name, string path) =>
            AllureApi.AddAttachment(path, name);
        public static void File(string fileName) => File(fileName, fileName);
    }
}