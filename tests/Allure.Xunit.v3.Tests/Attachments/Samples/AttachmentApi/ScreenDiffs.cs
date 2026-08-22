using System.IO;
using System.Threading.Tasks;
using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Attachments.AttachmentApi
{
    public class SyncScreenDiffs
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddScreenDiff(
                new byte[] { 1 },
                new byte[] { 2 },
                new byte[] { 3 }
            );

            var paths = CreatePaths();
            try
            {
                File.WriteAllBytes(paths.Expected, new byte[] { 4 });
                File.WriteAllBytes(paths.Actual, new byte[] { 5 });
                File.WriteAllBytes(paths.Diff, new byte[] { 6 });
                AllureApi.AddScreenDiffFromFiles(
                    paths.Expected,
                    paths.Actual,
                    paths.Diff
                );
            }
            finally
            {
                Delete(paths);
            }
        }

        internal static (string Expected, string Actual, string Diff) CreatePaths()
        {
            var prefix = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            return ($"{prefix}-expected.png", $"{prefix}-actual.png", $"{prefix}-diff.png");
        }

        internal static void Delete((string Expected, string Actual, string Diff) paths)
        {
            File.Delete(paths.Expected);
            File.Delete(paths.Actual);
            File.Delete(paths.Diff);
        }
    }

    public class AsyncScreenDiffs
    {
        [Fact]
        public async Task TestMethod()
        {
            var token = TestContext.Current.CancellationToken;
            await AllureApi.AddScreenDiffAsync(
                new byte[] { 11 },
                new byte[] { 12 },
                new byte[] { 13 },
                token
            );

            var paths = SyncScreenDiffs.CreatePaths();
            try
            {
                await File.WriteAllBytesAsync(paths.Expected, new byte[] { 14 }, token);
                await File.WriteAllBytesAsync(paths.Actual, new byte[] { 15 }, token);
                await File.WriteAllBytesAsync(paths.Diff, new byte[] { 16 }, token);
                await AllureApi.AddScreenDiffFromFilesAsync(
                    paths.Expected,
                    paths.Actual,
                    paths.Diff,
                    token
                );
            }
            finally
            {
                SyncScreenDiffs.Delete(paths);
            }
        }
    }
}
