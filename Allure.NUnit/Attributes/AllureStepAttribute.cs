using Allure.Net.Commons.Steps;

namespace Allure.NUnit.Attributes
{
    public class AllureStepAttribute : AllureStepAttributes.AbstractStepAttribute
    {
        public AllureStepAttribute(string name = null) : base(name)
        {
        }
    }
}