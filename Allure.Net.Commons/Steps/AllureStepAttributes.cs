using System;
using System.Collections.Generic;
using AspectInjector.Broker;

namespace Allure.Net.Commons.Steps;

public abstract class AllureStepAttributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public abstract class AbstractStepBaseAttribute : Attribute
    {
        protected AbstractStepBaseAttribute(string name, List<Type> exceptionTypes = null)
        {
            this.Name = name;
        }

        public string Name { get; protected set; }
    }
    
    [Injection(typeof(AllureStepAspectBase), Inherited = true)]
    public abstract class AbstractStepAttribute : AbstractStepBaseAttribute
    {
        protected AbstractStepAttribute(string name = null, List<Type> exceptionTypes = null) : base(name, exceptionTypes)
        {
        }
    }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    [Injection(typeof(AllureStepAspectBase), Inherited = true)]
    public abstract class AbstractBeforeAttribute : AbstractStepBaseAttribute
    {
        protected AbstractBeforeAttribute(string name = null, List<Type> exceptionTypes = null) : base(name, exceptionTypes)
        {
        }
    }
    
    [Injection(typeof(AllureStepAspectBase), Inherited = true)]
    public abstract class AbstractAfterAttribute : AbstractStepBaseAttribute
    {
        protected AbstractAfterAttribute(string name = null, List<Type> exceptionTypes = null) : base(name, exceptionTypes)
        {
        }
    }
}