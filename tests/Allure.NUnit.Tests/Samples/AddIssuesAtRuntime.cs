using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddIssuesAtRuntime
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddIssue("url-1");
        }

        [Test]
        public void TestMethod()
        {
            AllureApi.AddIssue("name-2", "url-2");
        }

        [TearDown]
        public void TearDown()
        {
            AllureApi.AddIssue("name-3", "url-3");
        }
    }
}
