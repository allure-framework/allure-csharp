using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.DefaultStepAttributes
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            this.Foo();
        }

        [AllureStep]
        void Foo() { }
    }
}

