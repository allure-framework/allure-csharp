using System;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Lifecycle.Outcomes
{
    public class TestClass
    {
        [Fact]
        public void PassingFact() { }

        [Fact]
        public void FailingFact()
        {
            Assert.Equal(1, 2);
        }

        [Fact]
        public void BrokenFact()
        {
            throw new InvalidOperationException("Something went wrong.");
        }

        [Fact(Skip = "Not part of this run.")]
        public void SkippedFact() { }
    }
}
