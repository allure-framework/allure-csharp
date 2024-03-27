using System;
using Allure.Net.Commons;

namespace Allure.Xunit
{
    public sealed class AllureStep : AllureStepBase<AllureStep>
    {
        [Obsolete("Use AllureStepAttribute")]
        public AllureStep(string name)
        {
            ExtendedApi.StartStep(name);
        }
    }
}