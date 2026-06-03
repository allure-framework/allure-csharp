using System;
using System.Collections.Generic;
using Xunit;

namespace Allure.Xunit.Tests.Parameters.Samples.TheoryWithThrowingMemberData
{
    public class TestsClass
    {
        public static IEnumerable<object[]> ThrowingData() =>
            throw new InvalidOperationException("Data source exploded!");

        [Theory]
        [MemberData(nameof(ThrowingData))]
        public void TestMethod(int _) { }
    }
}
