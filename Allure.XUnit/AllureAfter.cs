using System;
using Allure.Net.Commons.Steps;

namespace Allure.Xunit
{
    public sealed class AllureAfter : AllureStepBase<AllureAfter>
    {
        [Obsolete("Use AllureAfterAttribute")]
        public AllureAfter(string name)
        {
            CoreStepsHelper.StartAfterFixture(name);
        }
    }
}