using Allure.Commons.Writer;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Allure.Commons.Tests
{
    public class FileSystemResultsWriterTests
    {
        [Fact(DisplayName = "Should use temp path if no access to output directory")]
        public void ShouldCleanupTempResultsFolder()
        {
            var expectedDir = Path.Combine(Path.GetTempPath(), AllureConstants.DEFAULT_RESULTS_FOLDER);
            var moq = new Mock<FileSystemResultsWriter>(Environment.CurrentDirectory, true) { CallBase = true };
            moq.Setup(x => x.HasDirectoryAccess(It.IsAny<string>())).Returns(false);
            Assert.Equal(expectedDir, moq.Object.ResultsDirectory);
        }

        [Theory(DisplayName = "Cleanup logic for existing results directory")]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldCleanupExistingResultsFolder(bool cleanup)
        {
            var resultsDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(resultsDirectory);
            File.WriteAllText(Path.Combine(resultsDirectory, Path.GetRandomFileName()), "");

            var writer = new FileSystemResultsWriter(resultsDirectory, cleanup);

            if (cleanup)
                Assert.Empty(Directory.GetFiles(resultsDirectory));
            else
                Assert.NotEmpty(Directory.GetFiles(resultsDirectory));
        }
    }
}
