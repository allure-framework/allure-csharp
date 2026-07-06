using System;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Lifecycle.SingleBroken
{
    public class TestClass
    {
        [Fact]
        public void BrokenFact()
        {
            throw new InvalidOperationException("Something went wrong.");
        }
    }
}
