using Allure.Net.Commons.Steps;
using AspectInjector.Broker;

namespace NUnit.Allure.Attributes
{
    [Injection(typeof(Internals.StopContainerAspect), Inherited = true)]
    public class AllureAfterAttribute : AllureStepAttributes.AbstractAfterAttribute
    {
        public AllureAfterAttribute(string name = null) : base(name)
        {
        }
    }
}