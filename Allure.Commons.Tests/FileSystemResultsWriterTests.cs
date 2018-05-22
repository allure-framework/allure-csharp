using Allure.Commons.Writer;
using Moq;
using NUnit.Framework;
using System;
using System.IO;

namespace Allure.Commons.Tests
{
    [TestFixture]
    public class FileSystemResultsWriterTests
    {
        [Test, Description("Should use temp path if no access to output directory")]
        public void ShouldCleanupTempResultsFolder()
        {
            var expectedDir = Path.Combine(Path.GetTempPath(), AllureConstants.DEFAULT_RESULTS_FOLDER);
            var moq = new Mock<FileSystemResultsWriter>(Environment.CurrentDirectory) { CallBase = true };
            moq.Setup(x => x.HasDirectoryAccess(It.IsAny<string>())).Returns(false);
            Assert.AreEqual(expectedDir, moq.Object.ToString());
        }

        [Test, Description("Cleanup test")]
        public void Cleanup()
        {
            var resultsDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(resultsDirectory);
            File.WriteAllText(Path.Combine(resultsDirectory, Path.GetRandomFileName()), "");

            new FileSystemResultsWriter(resultsDirectory).CleanUp();
            Assert.IsEmpty(Directory.GetFiles(resultsDirectory));
        }
    }
}
