using System;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureStoryAttribute : AllureAttribute, IAllureInfo
    {
        public AllureStoryAttribute(string[] stories, bool overwrite = false)
        {
            Stories = stories;
            Overwrite = overwrite;
        }

        public AllureStoryAttribute(params string[] stories) : this(stories, false)
        {
        }

        internal string[] Stories { get; }
    }
}