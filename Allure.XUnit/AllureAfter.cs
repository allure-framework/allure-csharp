using System;
using Allure.Net.Commons;

namespace Allure.Xunit
{
    public sealed class AllureAfter : AllureStepBase<AllureAfter>
    {
        [Obsolete("Use AllureAfterAttribute")]
        public AllureAfter(string name)
        {
            ExtendedApi.StartAfterFixture(name);
        }
    }
}