using System;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Registration;
using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform;

/// <summary>
/// Provides registration helpers for the standalone Allure.TestingPlatform package.
/// </summary>
public static class AllureTestingPlatformExtensions
{
    /// <summary>
    /// Provides standalone Allure registration methods for a test application builder.
    /// </summary>
    /// <param name="builder">The test application builder.</param>
    extension (ITestApplicationBuilder builder)
    {
        /// <summary>
        /// Adds Allure.TestingPlatform to the test application and configures it.
        /// </summary>
        /// <param name="registration">A callback that configures the Allure runtime.</param>
        public void AddAllure(Action<IAllureTestingPlatformRegistrationContext> registration) =>
            AllureTestingPlatformSdkExtensions.RegisterAllureTestingPlatform(
                builder,
                "Allure.TestingPlatform",
                () => new AllureTestingPlatformRegistrationSession(),
                (context, serviceProvider) =>
                {
                    registration(context);
                }
            );

        /// <summary>
        /// Adds Allure.TestingPlatform to the test application with default settings.
        /// </summary>
        public void AddAllure() =>
            AddAllure(builder, static (_) => {});
    }
}
