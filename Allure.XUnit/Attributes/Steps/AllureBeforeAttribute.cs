using System;
using AspectInjector.Broker;

namespace Allure.XUnit.Attributes.Steps
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    [Injection(typeof(AllureStepAspect))]
    public class AllureBeforeAttribute : AllureStepBaseAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name)
        {
        }
    }
}