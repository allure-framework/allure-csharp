using System;
using Allure.XUnit.Attributes;

namespace Allure.Xunit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllureDescriptionAttribute : Attribute, IAllureInfo
    {
        public AllureDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}