using Allure.Net.Commons.Steps;
using Allure.Xunit;

namespace Allure.XUnit.Attributes.Steps
{
    public class AllureAfterAttribute : AllureStepAttributes.AbstractAfterAttribute
    {
        public AllureAfterAttribute(string name = null) : base(name)
        {
        }
    }
}