using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Allure.Commons.Writer
{
    class FileSystemResultsWriter : IAllureResultsWriter
    {
        private string outputDirectory;
        private JsonSerializer serializer = new JsonSerializer();

        public string Output => outputDirectory;

        internal FileSystemResultsWriter(string outputDirectory, bool cleanup)
        {
            this.outputDirectory = GetResultsDirectory(outputDirectory, cleanup);

            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;
            serializer.Converters.Add(new StringEnumConverter());
        }

        public void Write(TestResult testResult)
        {
            this.Write(testResult, AllureConstants.TEST_RESULT_FILE_SUFFIX);
        }
        public void Write(TestResultContainer testResult)
        {
            this.Write(testResult, AllureConstants.TEST_RESULT_CONTAINER_FILE_SUFFIX);
        }
        public void Write(string source, byte[] content)
        {
            var filePath = Path.Combine(outputDirectory, source);
            File.WriteAllBytes(filePath, content);
        }

        protected string Write(object allureObject, string fileSuffix)
        {
            dynamic obj = allureObject;
            var filePath = Path.Combine(outputDirectory, $"{obj.uuid}{fileSuffix}");
            using (StreamWriter fileStream = File.CreateText(filePath))
            {
                serializer.Serialize(fileStream, allureObject);
            }
            return filePath;
        }

        private string GetResultsDirectory(string outputDirectory, bool cleanup)
        {
            var tempFile = Path.Combine(outputDirectory, Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(outputDirectory);
                File.WriteAllText(tempFile, string.Empty);
                File.Delete(tempFile);
            }
            catch (UnauthorizedAccessException)
            {
                outputDirectory =
                    Directory.CreateDirectory(
                    Path.Combine(
                        Path.GetTempPath(), AllureConstants.DEFAULT_RESULTS_FOLDER))
                    .FullName;
            }
            finally
            {
                if (cleanup)
                    foreach (var file in new DirectoryInfo(outputDirectory).GetFiles())
                    {
                        file.Delete();
                    }
            }
            return new DirectoryInfo(outputDirectory).FullName;
        }
    }
}
