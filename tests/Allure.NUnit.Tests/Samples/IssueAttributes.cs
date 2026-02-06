using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.IssueAttributes
{
    [AllureIssue("url-1")]
    public interface IMetadata { }

    [AllureIssue("url-2", Title = "name-2")]
    public class TestClassBase { }

    [AllureNUnit]
    [AllureIssue("url-3")]
    public class TestsClass : TestClassBase, IMetadata
    {
        [Test]
        [AllureIssue("url-4")]
        public void TestMethod() { }
    }
}
