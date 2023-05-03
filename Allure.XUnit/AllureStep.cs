using System;
using Allure.Net.Commons.Steps;

namespace Allure.Xunit
{
    public sealed class AllureStep : AllureStepBase<AllureStep>
    {
        [Obsolete("Use AllureStepAttribute")]
        public AllureStep(string name) : base(Init(name))
        {
        }

        /// <summary>
        /// Creates a new step and return it's UUID
        /// </summary>
        /// <param name="name">The name of created step</param>
        /// <returns>string: UUID</returns>
        private static string Init(string name)
        {
            return CoreStepsHelper.StartStep(name);
        }
    }
}