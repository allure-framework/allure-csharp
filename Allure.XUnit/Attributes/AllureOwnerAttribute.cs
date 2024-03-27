using System;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureOwnerAttribute : AllureAttribute, IAllureInfo
    {
        public AllureOwnerAttribute(string owner, bool overwrite = false)
        {
            Owner = owner;
            Overwrite = overwrite;
        }

        internal string Owner { get; }
    }
}