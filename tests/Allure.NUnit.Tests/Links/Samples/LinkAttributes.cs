using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Links.Samples.LinkAttributes
{
    [AllureLink("url-1")]
    public interface IMetadata { }

    [AllureLink("url-2", Title = "name-2")]
    public class TestClassBase { }

    [AllureNUnit]
    [AllureLink("url-3", Type = "type-3")]
    public class TestsClass : TestClassBase, IMetadata
    {
        [Test]
        [AllureLink("url-4", Title = "name-4", Type = "type-4")]
        public void TestMethod() { }
    }
}
