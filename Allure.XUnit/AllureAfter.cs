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

        /// <summary>
        /// Starts After fixture and return it's UUID
        /// </summary>
        /// <param name="name">The name of created fixture</param>
        /// <returns>string: UUID</returns>
        private static string Init(string name)
        {
            return Steps.StartAfterFixture(name);
        }
    }
}