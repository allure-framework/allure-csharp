using Allure.Net.Commons.Steps;

namespace NUnit.Allure.Attributes
{
    public class AllureStepAttribute : AllureStepAttributes.AbstractStepAttribute
    {
        public AllureStepAttribute(string name = null) : base(name)
        {
        }
    }
}