using AspectInjector.Broker;

namespace Allure.XUnit.Attributes.Steps
{
    [Injection(typeof(AllureStepAspect))]
    public class AllureStepAttribute : AllureStepBaseAttribute
    {
        public AllureStepAttribute(string name = null) : base(name)
        {
        }
    }
}