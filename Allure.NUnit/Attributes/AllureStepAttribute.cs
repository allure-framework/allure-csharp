using Allure.Net.Commons.Steps;
using NUnit.Allure.Core;

namespace NUnit.Allure.Attributes
{
    public class AllureStepAttribute : AllureStepAttributes.AbstractStepAttribute
    {
        public AllureStepAttribute(string name = null) : base(name, AllureNUnitHelper.ExceptionTypes)
        {
        }
    }
}