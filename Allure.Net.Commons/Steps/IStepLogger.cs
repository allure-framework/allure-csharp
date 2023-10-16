namespace Allure.Net.Commons.Steps
{
    public interface IStepLogger
    {
        IStepActionLogger StepStarted { get; set; }
        IStepActionLogger StepPassed { get; set; }
        IStepActionLogger StepFailed { get; set; }
        IStepActionLogger StepBroken { get; set; }
        IStepActionLogger BeforeStarted { get; set; }
        IStepActionLogger AfterStarted { get; set; }
    }

    public interface IStepActionLogger
    {
        /// <summary>
        /// Log step action
        /// </summary>
        /// <param name="name">Name of step</param>
        void Log(string name);
    }
}
