using Allure.Runtime;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Sets the name of the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="newName">The new name of the test.</param>
    public static void SetTestName(string newName) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetTestName(newName);

    /// <summary>
    /// Sets the name of the current fixture.
    /// </summary>
    /// <param name="newName">The new name of the fixture.</param>
    public static void SetFixtureName(string newName) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetFixtureName(newName);

    /// <summary>
    /// Sets the name of the current step, fixture, or test.
    /// </summary>
    /// <param name="newName">A new name.</param>
    /// <remarks>
    /// If a step is running, renames the step.
    /// Otherwise, if a fixture is running, renames the fixture.
    /// Otherwise, if a test is running, renames the test.
    /// Otherwise, does nothing.
    /// </remarks>
    public static void SetName(string newName) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetName(newName);
}
