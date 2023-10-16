using Allure.Net.Commons.Steps;
using NUnit.Allure.Core;

namespace NUnit.Allure.Attributes
{
    public class AllureBeforeAttribute : AllureStepAttributes.AbstractBeforeAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name, AllureNUnitHelper.ExceptionTypes)
        {
        }
    }
}