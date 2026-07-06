using System;
using System.Collections.Generic;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Lifecycle.Failures
{
    public class BadClass
    {
        public BadClass()
        {
            throw new InvalidOperationException("Constructor exploded.");
        }

        [Fact]
        public void TestMethod() { }
    }

    public class GoodClass
    {
        public static IEnumerable<object[]> ThrowingData() =>
            throw new InvalidOperationException("Data source exploded.");

        [Theory]
        [MemberData(nameof(ThrowingData))]
        public void BadTheory(string value)
        {
            _ = value;
        }
    }
}
