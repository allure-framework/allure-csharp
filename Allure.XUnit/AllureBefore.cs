using System;
using Allure.Net.Commons.Steps;

namespace Allure.Xunit
{
    public sealed class AllureBefore : AllureStepBase<AllureBefore>
    {
        [Obsolete("Use AllureBeforeAttribute")]
        public AllureBefore(string name)
        {
            CoreStepsHelper.StartBeforeFixture(name);
        }
    }
}