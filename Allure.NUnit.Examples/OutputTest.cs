using System;
using System.IO;
using System.Linq;
using Allure.Net.Commons;
using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Output")]
    public class OutputTest : BaseTest
    {
        private const string text = "This should go to console output attachment";

        [Test]
        [Order(1)]
        public void WriteOutputTest()
        {
            Console.WriteLine(text);
        }

        [Test]
        [Order(2)]
        public void OutputLogShouldExist()
        {
            var resultsDir = AllureLifecycle.Instance.ResultsDirectory;
            var attachmentFiles = Directory.EnumerateFiles(resultsDir, "*.txt");
            
            Assert.That(attachmentFiles.Any(file => File.ReadAllText(file).Contains(text)));
        }
    }
}

