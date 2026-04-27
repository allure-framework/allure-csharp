using System;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureFeatureAttribute : AllureAttribute, IAllureInfo
    {
        public AllureFeatureAttribute(string[] features, bool overwrite = false)
        {
            Features = features;
            Overwrite = overwrite;
        }

        public AllureFeatureAttribute(params string[] features) : this(features, false)
        {
        }

        internal string[] Features { get; }
    }
}