using Allure.Net.Commons.Steps;

namespace Allure.NUnit.Attributes
{
    public class AllureBeforeAttribute : AllureStepAttributes.AbstractBeforeAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name)
        {
        }
    }
}