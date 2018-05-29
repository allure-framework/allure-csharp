using System;

namespace Allure.Commons.Helpers
{
    internal static class DateTimeHelper
    {
        public static long ToUnixTimeMilliseconds(this DateTimeOffset dateTimeOffset)
        {
            return (long) dateTimeOffset.UtcDateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}