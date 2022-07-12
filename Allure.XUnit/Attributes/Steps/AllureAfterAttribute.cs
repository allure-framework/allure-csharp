using AspectInjector.Broker;

namespace Allure.XUnit.Attributes.Steps
{
    [Injection(typeof(AllureStepAspect))]
    public class AllureAfterAttribute : AllureStepBaseAttribute
    {
        public AllureAfterAttribute(string name = null) : base(name)
        {
        }
    }
}