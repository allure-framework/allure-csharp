using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Writer;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Allure.Net.Commons.Tests
{
    [TestFixture]
    public class FileSystemResultsWriterTests
    {
        [Test, Description("Should use temp path if no access to output directory")]
        public void ShouldUseTempPathIfNoAccessToResultsDirectory()
        {
            var config = AllureConfiguration.ReadFromJObject(JObject.Parse(@"{allure:{}}"));
            var expectedDir = Path.Combine(Path.GetTempPath(), AllureConstants.DEFAULT_RESULTS_FOLDER);
            var moq = new Mock<FileSystemResultsWriter>(config) { CallBase = true };
            moq.Setup(x => x.HasDirectoryAccess(It.IsAny<string>())).Returns(false);
            Assert.AreEqual(expectedDir, moq.Object.ToString());
        }

        [Test, Description("Cleanup test")]
        public void ShouldCleanupTempResultsFolder()
        {
            var resultsDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var json = $"{{\"allure\":{{\"directory\": {JsonConvert.ToString(resultsDirectory)}}}}}";
            var config = AllureConfiguration.ReadFromJObject(JObject.Parse(json));
            Directory.CreateDirectory(resultsDirectory);
            File.WriteAllText(Path.Combine(resultsDirectory, Path.GetRandomFileName()), "");

            new FileSystemResultsWriter(config).CleanUp();
            Assert.IsEmpty(Directory.GetFiles(resultsDirectory));
        }
    }
}
