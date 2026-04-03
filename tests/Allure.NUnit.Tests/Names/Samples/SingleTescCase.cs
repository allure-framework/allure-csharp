using NUnit.Framework;

namespace Allure.NUnit.Tests.Names.Samples.LegacyNameAttribute
{
    [AllureNUnit]
    public class TestsClass
    {
        [TestCase(1)]
        public void TestMethod(int _) { }
    }
}
