using System;
using NUnit.Framework.Interfaces;

namespace Allure.NUnit.Core
{
    static class AllureExtensions
    {
        internal static long ToUnixTimeMilliseconds(this DateTimeOffset dateTimeOffset)
        {
            return (long) dateTimeOffset.UtcDateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        internal const string DESELECTED_BY_TESTPLAN_KEY
            = "DESELECTED_BY_TESTPLAN";

        static internal void Deselect(this ITest test) =>
            test.Properties.Add(DESELECTED_BY_TESTPLAN_KEY, true);

        static internal bool IsDeselected(this ITest test) =>
            test.Properties.ContainsKey(DESELECTED_BY_TESTPLAN_KEY);
    }
}