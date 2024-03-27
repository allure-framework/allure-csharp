using System;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureTagAttribute : AllureAttribute, IAllureInfo
    {
        public AllureTagAttribute(string[] tags, bool overwrite = false)
        {
            Tags = tags;
            Overwrite = overwrite;
        }

        public AllureTagAttribute(params string[] tags) : this(tags, false)
        {
        }

        internal string[] Tags { get; }
    }
}