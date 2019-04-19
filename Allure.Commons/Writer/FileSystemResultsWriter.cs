using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Allure.Commons.Configuration;
using Allure.Commons.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[assembly: InternalsVisibleTo("Allure.Commons.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Allure.Commons.Writer
{
    internal class FileSystemResultsWriter : IAllureResultsWriter
    {
        private readonly AllureConfiguration configuration;
        //private Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string outputDirectory;
        private readonly JsonSerializer serializer = new JsonSerializer();

        internal FileSystemResultsWriter(AllureConfiguration configuration)
        {
            this.configuration = configuration;
            outputDirectory = GetResultsDirectory(configuration.Directory);

            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;
            serializer.Converters.Add(new StringEnumConverter());
        }

        public void Write(TestResult testResult)
        {
            LinkHelper.UpdateLinks(testResult.links, configuration.Links);
            Write(testResult, AllureConstants.TEST_RESULT_FILE_SUFFIX);
        }

        public void Write(TestResultContainer testResult)
        {
            LinkHelper.UpdateLinks(testResult.links, configuration.Links);
            Write(testResult, AllureConstants.TEST_RESULT_CONTAINER_FILE_SUFFIX);
        }

        public void Write(string source, byte[] content)
        {
            var filePath = Path.Combine(outputDirectory, source);
            File.WriteAllBytes(filePath, content);
        }

        public void CleanUp()
        {
            using (var mutex = new Mutex(false, "729dc988-0e9c-49d0-9e50-17e0df3cd82b"))
            {
                mutex.WaitOne();
                var directory = new DirectoryInfo(outputDirectory);
                foreach (var file in directory.GetFiles()) file.Delete();
                mutex.ReleaseMutex();
            }
        }

        public override string ToString()
        {
            return outputDirectory;
        }

        protected string Write(object allureObject, string fileSuffix)
        {
            var filePath = Path.Combine(outputDirectory, $"{Guid.NewGuid().ToString("N")}{fileSuffix}");
            using (var fileStream = File.CreateText(filePath))
            {
                serializer.Serialize(fileStream, allureObject);
            }

            return filePath;
        }

        internal virtual bool HasDirectoryAccess(string directory)
        {
            var tempFile = Path.Combine(directory, Guid.NewGuid().ToString());
            try
            {
                File.WriteAllText(tempFile, string.Empty);
                File.Delete(tempFile);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private string GetResultsDirectory(string outputDirectory)
        {
            var parentDir = new DirectoryInfo(outputDirectory).Parent.FullName;
            outputDirectory = HasDirectoryAccess(parentDir)
                ? outputDirectory
                : Path.Combine(
                    Path.GetTempPath(), AllureConstants.DEFAULT_RESULTS_FOLDER);

            Directory.CreateDirectory(outputDirectory);

            return new DirectoryInfo(outputDirectory).FullName;
        }
    }
}