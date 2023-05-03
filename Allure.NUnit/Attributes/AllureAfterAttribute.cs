using Allure.Net.Commons.Steps;
using NUnit.Allure.Core;

namespace NUnit.Allure.Attributes
{
    public class AllureAfterAttribute : AllureStepAttributes.AbstractAfterAttribute
    {
        public AllureAfterAttribute(string name = null) : base(name, AllureNUnitHelper.ExceptionTypes)
        {
        }
    }
}