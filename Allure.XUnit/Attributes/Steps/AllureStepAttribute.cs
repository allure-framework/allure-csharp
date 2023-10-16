using Allure.Net.Commons.Steps;
using Allure.Xunit;

namespace Allure.XUnit.Attributes.Steps
{
    public class AllureStepAttribute : AllureStepAttributes.AbstractStepAttribute
    {
        public AllureStepAttribute(string name = null) : base(name, AllureXunitHelper.ExceptionTypes)
        {
        }
    }
}