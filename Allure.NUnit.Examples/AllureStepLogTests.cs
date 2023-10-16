using System;
using Allure.Net.Commons.Steps;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Allure.NUnit.Examples
{
    class StepActionLogger : IStepActionLogger
    {
        private readonly string _prefix;

        public StepActionLogger(string prefix)
        {
            _prefix = prefix;
        }
        
        public void Log(string name)
        {
            Console.WriteLine(_prefix + ": " + name);
        }
    }

    class StepActionLoggerNoName : IStepActionLogger
    {
        private readonly string _message;

        public StepActionLoggerNoName(string message)
        {
            _message = message;
        }
        
        public void Log(string name)
        {
            Console.WriteLine(_message);
        }
    }
    
    class StepsLogger : IStepLogger
    {
        public IStepActionLogger StepStarted { get; set; } = new StepActionLogger("Started Step");
        public IStepActionLogger StepPassed { get; set; } = new StepActionLoggerNoName("Passed");
        public IStepActionLogger StepFailed { get; set; } = new StepActionLoggerNoName("Failed");
        public IStepActionLogger StepBroken { get; set; } = new StepActionLoggerNoName("Broken");
        public IStepActionLogger BeforeStarted { get; set; } = null;
        public IStepActionLogger AfterStarted { get; set; } = null;
    }
    
    [AllureSuite("Tests - Step Logs")]
    public class AllureStepLogTests : BaseTest
    {
        [OneTimeSetUp]
        public void SetStepLogger()
        {
            StepsHelper.StepLogger = new StepsLogger();
        }

        [Test]
        public void LoggerTest()
        {
            SomeAction();

            var testOutput = TestExecutionContext.CurrentContext.CurrentResult.Output;
            
            Assert.That(testOutput, Does.Contain("Started Step: Some action"));
            Assert.That(testOutput, Does.Contain("Passed"));
        }

        [AllureStep("Some action")]
        public void SomeAction()
        {
            Console.WriteLine("Executing step action");
        }
    }
}