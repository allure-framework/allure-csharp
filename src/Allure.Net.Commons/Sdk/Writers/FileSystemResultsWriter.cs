using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Allure.Net.Commons.Sdk.Writers
{
    /// <summary>
    /// Writer that emits results to an output directory.
    /// </summary>
    public class FileSystemResultsWriter : IAllureResultsWriter
    {
        private readonly string outputDirectory;
        private readonly JsonSerializer serializer = new();

        public FileSystemResultsWriter(string outputDirectory, bool indentOutput)
        {
            this.outputDirectory = outputDirectory;

            serializer.NullValueHandling = NullValueHandling.Ignore;
            if (indentOutput)
            {
                serializer.Formatting = Formatting.Indented;
            }

            serializer.Converters.Add(
                new StringEnumConverter(
                    new CamelCaseNamingStrategy()
                )
            );
        }

        public void Write(TestResult testResult)
        {
            Write(testResult, AllureConstants.TEST_RESULT_FILE_SUFFIX);
        }

        public void Write(TestResultContainer testResult)
        {
            Write(testResult, AllureConstants.TEST_RESULT_CONTAINER_FILE_SUFFIX);
        }

        public void Write(Globals globals)
        {
            Write(globals, AllureConstants.GLOBALS_FILE_SUFFIX);
        }

        public void Write(string outputFileName, byte[] content)
        {
            var outputFilePath = Path.Combine(outputDirectory, outputFileName);

            this.EnsureDirectory();

            File.WriteAllBytes(outputFilePath, content);
        }

        public void Write(string destinationFileName, string sourceFilePath)
        {
            var destinationPath = Path.Combine(outputDirectory, destinationFileName);

            this.EnsureDirectory();

            File.Copy(sourceFilePath, destinationPath);
        }

        public void CleanUp()
        {
            var directory = new DirectoryInfo(outputDirectory);
            if (!directory.Exists)
            {
                return;
            }

            using var mutex = new Mutex(false, "729dc988-0e9c-49d0-9e50-17e0df3cd82b");
            mutex.WaitOne();

            foreach (var file in directory.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in directory.GetDirectories())
            {
                dir.Delete(true);
            }
            mutex.ReleaseMutex();
        }

        string Write(object allureObject, string fileSuffix)
        {
            var uuid = Guid.NewGuid();
            var filePath = Path.Combine(outputDirectory, $"{uuid:N}{fileSuffix}");

            this.EnsureDirectory();

            using (var fileStream = File.CreateText(filePath))
            {
                serializer.Serialize(fileStream, allureObject);
            }

            return filePath;
        }

        void EnsureDirectory()
        {
            if (!Directory.Exists(this.outputDirectory))
            {
                Directory.CreateDirectory(this.outputDirectory);
            }
        }
    }
}