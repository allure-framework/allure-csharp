using AspectInjector.Broker;

namespace Allure.XUnit.Attributes.Steps
{
    [Injection(typeof(AllureStepAspect))]
    public class AllureBeforeAttribute : AllureStepBaseAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name)
        {
        }
    }
}