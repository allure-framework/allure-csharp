using System;
using System.IO;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Attachments")]
    public class AllureAttachmentsTest : BaseTest
    {
        [Test]
        public void AttachmentSimpleTest()
        {
            Console.WriteLine("With Attachment");
            Console.WriteLine(DateTime.Now);

            Attachments.File("AllureConfig.json", Path.Combine(TestContext.CurrentContext.TestDirectory, "allureConfig.json"));
        }
    }
}