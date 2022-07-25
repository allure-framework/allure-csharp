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

        /// <summary>
        /// Starts Before fixture and return it's UUID
        /// </summary>
        /// <param name="name">The name of created fixture</param>
        /// <returns>string: UUID</returns>
        private static string Init(string name)
        {
            return Steps.StartBeforeFixture(name);
        }
    }
}