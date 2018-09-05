using Allure.Commons.Configuration;
using Allure.Commons.Writer;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;

namespace Allure.Commons.Tests
{
    [TestFixture]
    public class FileSystemResultsWriterTests
    {
        [Test, Description("Should use temp path if no access to output directory")]
        public void ShouldUseTempPathIfNoAccessToResultsDirectory()
        {
            var config = AllureConfiguration.ReadFromJson(@"{allure:{}}");
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
            var config = AllureConfiguration.ReadFromJson(json);
            Directory.CreateDirectory(resultsDirectory);
            File.WriteAllText(Path.Combine(resultsDirectory, Path.GetRandomFileName()), "");

            new FileSystemResultsWriter(config).CleanUp();
            Assert.IsEmpty(Directory.GetFiles(resultsDirectory));
        }
    }
}
