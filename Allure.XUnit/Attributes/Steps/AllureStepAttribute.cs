using Allure.Net.Commons.Steps;

namespace Allure.XUnit.Attributes.Steps
{
    public class AllureStepAttribute : AllureStepAttributes.AbstractStepAttribute
    {
        public AllureStepAttribute(string name = null) : base(name)
        {
        }
    }
}