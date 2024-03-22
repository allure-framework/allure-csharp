using Allure.Net.Commons.Steps;

namespace Allure.XUnit.Attributes.Steps
{
    public class AllureBeforeAttribute : AllureStepAttributes.AbstractBeforeAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name)
        {
        }
    }
}