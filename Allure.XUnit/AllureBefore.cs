using System;
using Allure.Net.Commons;

namespace Allure.Xunit
{
    public sealed class AllureBefore : AllureStepBase<AllureBefore>
    {
        [Obsolete("Use AllureBeforeAttribute")]
        public AllureBefore(string name) : base(Init(name))
        {
        }

        private static ExecutableItem Init(string name)
        {
            Steps.StartBeforeFixture(name);
            return Steps.Current;
        }
    }
}