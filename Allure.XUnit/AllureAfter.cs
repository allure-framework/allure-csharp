using System;
using Allure.Net.Commons;

namespace Allure.Xunit
{
    public sealed class AllureAfter : AllureStepBase<AllureAfter>
    {
        [Obsolete("Use AllureAfterAttribute")]
        public AllureAfter(string name) : base(Init(name))
        {
        }

        private static ExecutableItem Init(string name)
        {
            Steps.StartAfterFixture(name);
            return Steps.Current;
        }
    }
}