using System;
using Allure.XUnit.Attributes;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureParentSuiteAttribute : AllureAttribute, IAllureInfo
    {
        public AllureParentSuiteAttribute(string parentSuite, bool overwrite = false)
        {
            ParentSuite = parentSuite;
            Overwrite = overwrite;
        }

        internal string ParentSuite { get; }
    }
}