// using System;

using System;
using System.Runtime.Serialization;

namespace Allure.Net.Commons.Steps
{
    [Serializable]
    public class StepFailedException : Exception
    {
        protected StepFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        
        public StepFailedException(string stepName, Exception inner) : base($"Step failed: {stepName}", inner)
        {
        }
    }
}