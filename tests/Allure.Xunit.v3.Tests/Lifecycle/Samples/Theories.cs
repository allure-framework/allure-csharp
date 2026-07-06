using System;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Lifecycle.Theories
{
    public class TestClass
    {
        [Theory]
        [InlineData("first")]
        [InlineData("second")]
        [InlineData("third")]
        public void TestMethod(string value)
        {
            Assert.NotEmpty(value);
        }
    }
}
