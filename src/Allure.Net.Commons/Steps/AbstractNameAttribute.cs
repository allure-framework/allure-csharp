using System;

namespace Allure.Net.Commons.Steps
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class AbstractNameAttribute : Attribute
    {
        protected AbstractNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}