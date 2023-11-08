using System.Text;
using HeyRed.Mime;

namespace Allure.XUnit
{
    public abstract class Attachments
    {
        public static void Text(string name, string content) => Bytes(name, Encoding.UTF8.GetBytes(content), ".txt");
        public static void Bytes(string name, byte[] content, string extension = "") =>
            Net.Commons.Allure.AddAttachment(name, MimeTypesMap.GetMimeType(extension), content, extension);
        public static void File(string name, string path) =>
            Net.Commons.Allure.AddAttachment(path, name);
        public static void File(string fileName) => File(fileName, fileName);
    }
}