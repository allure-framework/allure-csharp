namespace NUnit.Allure.Core
{
    public interface IStepNotifier
    {
        INotify StepStarted { get; set; }
        INotify StepPassed { get; set; }
        INotify StepFailed { get; set; }
        INotify StepBroken { get; set; }
        INotify BeforeStarted { get; set; }
        INotify AfterStarted { get; set; }
    }

    public interface INotify
    {
        void Log(string name);
    }
}