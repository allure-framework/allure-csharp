using System;
using Allure.Net.Commons;

namespace Allure.Xunit
{
    public sealed class AllureStep : AllureStepBase<AllureStep>
    {
        [Obsolete("Use AllureStepAttribute")]
        public AllureStep(string name) : base(Init(name))
        {
        }

        private static ExecutableItem Init(string name)
        {
            Steps.StartStep(name);
            return Steps.Current;
        }
    }
}