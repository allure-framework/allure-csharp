using System;
using Allure.Net.Commons.Steps;

namespace Allure.Xunit
{
    public sealed class AllureStep : AllureStepBase<AllureStep>
    {
        [Obsolete("Use AllureStepAttribute")]
        public AllureStep(string name)
        {
            CoreStepsHelper.StartStep(name);
        }
    }
}