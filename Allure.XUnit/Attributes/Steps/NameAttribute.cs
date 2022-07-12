using System;

namespace Allure.XUnit.Attributes.Steps
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}