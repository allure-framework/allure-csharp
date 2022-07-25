using System.IO;
using System.Text;
using Allure.Net.Commons;
using HeyRed.Mime;

namespace Allure.XUnit
{
    public class Attachments
    {
        public static void Text(string name, string content) =>
            Bytes(name, Encoding.UTF8.GetBytes(content), ".txt");
        
        public static void Bytes(string name, byte[] content, string extension = "") =>
            AddAttachment(name, MimeTypesMap.GetMimeType(extension), content, extension);
        
        public static void File(string fileName) =>
            File(fileName, fileName);

        public static void File(string name, string path) =>
            AllureLifecycle.Instance.AddAttachment(path, name);

        private static void AddAttachment(string name, string type, byte[] content, string fileExtension) =>
            AllureLifecycle.Instance.AddAttachment(name, type, content, fileExtension);
    }
}