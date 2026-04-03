using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Links.Samples.TmsItemAttributes
{
    [AllureTmsItem("url-1")]
    public interface IMetadata { }

    [AllureTmsItem("url-2", Title = "name-2")]
    public class TestClassBase { }

    [AllureNUnit]
    [AllureTmsItem("url-3")]
    public class TestsClass : TestClassBase, IMetadata
    {
        [Test]
        [AllureTmsItem("url-4")]
        public void TestMethod() { }
    }
}
