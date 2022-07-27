using System;

namespace NUnit.Allure.Core
{
    public class StepFailedException : Exception
    {
        public StepFailedException(string stepName, Exception inner) : base($"Step failed: {stepName}", inner)
        {
        }
    }
}