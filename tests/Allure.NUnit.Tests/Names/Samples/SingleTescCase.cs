using NUnit.Framework;

namespace Allure.NUnit.Tests.Names.Samples.SingleTescCase
{
    [AllureNUnit]
    public class TestsClass
    {
        [TestCase(1)]
        public void TestMethod(int _) { }
    }
}
