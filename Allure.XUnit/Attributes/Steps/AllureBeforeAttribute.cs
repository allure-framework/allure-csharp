using Allure.Net.Commons.Steps;
using Allure.Xunit;

namespace Allure.XUnit.Attributes.Steps
{
    public class AllureBeforeAttribute : AllureStepAttributes.AbstractBeforeAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name, AllureXunitHelper.ExceptionTypes)
        {
        }
    }
}