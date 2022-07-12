using System;

namespace Allure.XUnit.Attributes.Steps
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class AllureStepBaseAttribute : Attribute
    {
        protected AllureStepBaseAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }
    }
}