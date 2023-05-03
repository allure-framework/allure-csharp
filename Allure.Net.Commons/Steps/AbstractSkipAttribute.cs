using System;

namespace Allure.Net.Commons.Steps
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class AbstractSkipAttribute : Attribute
    {
    }
}