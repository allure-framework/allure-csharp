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
        DirectoryInfo tmpDir;

        [SetUp]
        public void CreateTempDir()
        {
            this.tmpDir = Directory.CreateTempSubdirectory();
        }

        [TearDown]
        public void DeleteTmpDir()
        {
            if (this.tmpDir.Exists)
            {
                try
                {
                    this.tmpDir.Delete(true);
                }
                catch (Exception e)
                {
                    TestContext.WriteLine(e.ToString());
                }
            }
        }

        [Test, Description("Should use temp path if no access to output directory")]
        public void ShouldUseTempPathIfNoAccessToResultsDirectory()
        {
            var config = AllureConfiguration.ReadFromJObject(JObject.Parse(@"{allure:{}}"));
            var expectedDir = Path.Combine(Path.GetTempPath(), AllureConstants.DEFAULT_RESULTS_FOLDER);
            var moq = new Mock<FileSystemResultsWriter>(config) { CallBase = true };
            moq.Setup(x => x.HasDirectoryAccess(It.IsAny<string>())).Returns(false);
            Assert.That(moq.Object.ToString(), Is.EqualTo(expectedDir));
        }

        [Test, Description("Cleanup test")]
        public void ShouldCleanupTempResultsFolder()
        {
            var json = $"{{\"allure\":{{\"directory\": {JsonConvert.ToString(this.tmpDir.FullName)}}}}}";
            var config = AllureConfiguration.ReadFromJObject(JObject.Parse(json));
            File.WriteAllText(Path.Combine(this.tmpDir.FullName, Path.GetRandomFileName()), "");

            new FileSystemResultsWriter(config).CleanUp();
            Assert.That(this.tmpDir.EnumerateFiles(), Is.Empty);
        }
    }
}
