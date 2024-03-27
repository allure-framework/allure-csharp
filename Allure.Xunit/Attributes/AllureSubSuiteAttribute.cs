using System;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureSubSuiteAttribute : AllureAttribute, IAllureInfo
    {
        public AllureSubSuiteAttribute(string subSuite, bool overwrite = false)
        {
            SubSuite = subSuite;
            Overwrite = overwrite;
        }

        internal string SubSuite { get; }
    }
}