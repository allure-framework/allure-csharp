using System.Text;
using HeyRed.Mime;

using AllureUserAPI = Allure.Net.Commons.Allure;

namespace NUnit.Allure
{
    public abstract class Attachments
    {
        public static void Text(string name, string content) => Bytes(name, Encoding.UTF8.GetBytes(content), ".txt");
        public static void Bytes(string name, byte[] content, string extension = "") =>
            AllureUserAPI.AddAttachment(name, MimeTypesMap.GetMimeType(extension), content, extension);
        public static void File(string name, string path) =>
            AllureUserAPI.AddAttachment(path, name);
        public static void File(string fileName) => File(fileName, fileName);
    }
}