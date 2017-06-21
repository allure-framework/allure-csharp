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
        JsonSerializer serializer = new JsonSerializer();
        internal FileSystemResultsWriter(string outputDirectory, bool cleanup)
        {
            if (cleanup && Directory.Exists(outputDirectory))
                foreach (var file in new DirectoryInfo(outputDirectory).GetFiles())
                {
                    file.Delete();
                }

            Directory.CreateDirectory(outputDirectory);

            this.outputDirectory = outputDirectory;
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

        public void Write(string source, Stream attachment)
        {
            throw new NotImplementedException();
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
    }
}
