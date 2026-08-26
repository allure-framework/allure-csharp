using System.Threading;

namespace Allure.Xunit.Registration;

/// <summary>
/// Provides access to the Allure.Xunit.v3 registration hook configured
/// programmatically.
/// </summary>
public static class AllureXunitRegistrationHook
{
    static IAllureXunitRegistrationHook? currentHook = null;

    /// <summary>
    /// Gets or sets the registration hook configured for the current test application.
    /// </summary>
    /// <value>
    /// The hook to apply during Allure.Xunit.v3 registration, or <see langword="null"/>
    /// if no programmatic hook is configured.
    /// </value>
    /// <remarks>
    /// Assign this property before Allure.Xunit.v3 registration begins. Changing it
    /// does not reconfigure a registration that has already completed.
    /// </remarks>
    public static IAllureXunitRegistrationHook? Current
    {
        get => Volatile.Read(ref currentHook);
        set => Volatile.Write(ref currentHook, value);
    }
}
