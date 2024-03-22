using Allure.Net.Commons.Steps;

namespace NUnit.Allure.Attributes
{
    public class AllureBeforeAttribute : AllureStepAttributes.AbstractBeforeAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name)
        {
        }
    }
}