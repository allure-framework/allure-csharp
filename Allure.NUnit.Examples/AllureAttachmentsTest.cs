using System;
using System.IO;
using Allure.Net.Commons;
using NUnit.Allure.Attributes;
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


            AllureLifecycle.Instance.AddAttachment(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "allureConfig.json"),
                "AllureConfig.json");
        }
    }
}