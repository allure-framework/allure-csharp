using System;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureSuiteAttribute : AllureAttribute, IAllureInfo
    {
        public AllureSuiteAttribute(string suite, bool overwrite = false)
        {
            Suite = suite;
            Overwrite = overwrite;
        }

        internal string Suite { get; }
    }
}