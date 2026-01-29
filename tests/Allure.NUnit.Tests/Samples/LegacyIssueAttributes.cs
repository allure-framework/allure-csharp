using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacyIssueAttributes
{
    [AllureIssue("url-1")]
    public class TestClassBase { }

    [AllureNUnit]
    [AllureIssue("name-2", "url-2")]
    public class TestsClass : TestClassBase
    {
        [Test]
        [AllureIssue("url-3")]
        public void TestMethod() { }
    }
}
