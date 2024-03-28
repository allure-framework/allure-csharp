using System;
using System.Collections;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Parametrized")]
    public class AllureParametrizedTest : BaseTest
    {
        public static IEnumerable TestCasesReturns
        {
            get
            {
                yield return new TestCaseData(12, 3).Returns(4);
                yield return new TestCaseData(12, 2).Returns(6);
                yield return new TestCaseData(12, 4).Returns(3);
            }
        }

        [Test]
        [AllureSubSuite("Returns")]
        [AllureTag("TestCaseSource")]
        [TestCaseSource(typeof(AllureParametrizedTest), nameof(TestCasesReturns))]
        public int TestCaseSourceTest(int n, int d)
        {
            Console.WriteLine($"Validating {n} / {d} ");
            return n / d;
        }

        [Test]
        [Pairwise]
        [AllureSubSuite("Pairwise")]
        [AllureTag("Pairwise")]
        public void PairwiseTest([Values("a", "b", "c")] string str)
        {
            Console.WriteLine(str);
        }

        [Test]
        [Retry(5)]
        [AllureSubSuite("Retry")]
        [AllureTag("Retry")]
        public void RetryTest()
        {
        }

        [Test]
        [AllureTag("Range")]
        [AllureSubSuite("Range")]
        public void RangeTest([Range(0, 2)] int value)
        {
            Console.WriteLine($"Got {value} from 0 to 2");
        }
    }
}