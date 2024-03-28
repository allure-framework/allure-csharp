using System;
using Allure.Net.Commons;

namespace Allure.Xunit
{
    public sealed class AllureBefore : AllureStepBase<AllureBefore>
    {
        [Obsolete("Use AllureBeforeAttribute")]
        public AllureBefore(string name)
        {
            ExtendedApi.StartBeforeFixture(name);
        }
    }
}