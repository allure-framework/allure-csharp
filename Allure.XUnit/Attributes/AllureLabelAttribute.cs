using System;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureLabelAttribute : AllureAttribute, IAllureInfo
    {
        public AllureLabelAttribute(string label, string value, bool overwrite = false)
        {
            Label = label;
            Value = value;
            Overwrite = overwrite;
        }

        public string Label { get; }
        public string Value { get; }
    }
}