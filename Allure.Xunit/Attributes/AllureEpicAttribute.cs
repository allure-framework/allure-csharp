using System;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureEpicAttribute : AllureAttribute, IAllureInfo
    {
        public AllureEpicAttribute(string epic, bool overwrite = false)
        {
            Epic = epic;
            Overwrite = overwrite;
        }

        public string Epic { get; }
    }
}