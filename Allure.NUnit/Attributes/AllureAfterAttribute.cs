using AspectInjector.Broker;
using NUnit.Allure.Core.Steps;

namespace NUnit.Allure.Attributes
{
    [Injection(typeof(AllureStepAspect))]
    public class AllureAfterAttribute : AllureStepBaseAttribute
    {
        public AllureAfterAttribute(string name = null) : base(name)
        {
        }
    }
}