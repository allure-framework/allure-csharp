using System;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform;

/// <summary>
/// Provides registration helpers for the standalone Allure.TestingPlatform package.
/// </summary>
public static class AllureTestingPlatformExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        /// <summary>
        /// Adds Allure.TestingPlatform to the test application and configures it.
        /// </summary>
        public void AddAllure(Action<IAllureTestingPlatformRuntimeRegistrationContext> registration) =>
            AllureTestingPlatformSdkExtensions.RegisterAllureTestingPlatform<
                AllureTestingPlatformConfiguration,
                IAllureTestingPlatformRuntimeRegistrationContext,
                IAllureTestingPlatformRuntimeRegistrationHook,
                IAllureTestingPlatformEndpointRegistrationContext,
                IAllureTestingPlatformEndpointRegistrationHook,
                IAllureTestingPlatformRuntimeIntegrationContext,
                IAllureRuntimeIntegrationSnapshot<
                    AllureTestingPlatformConfiguration,
                    IAllureTestingPlatformEndpointRegistrationContext,
                    IAllureTestingPlatformEndpointRegistrationHook,
                    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
                >,
                IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
            >(
                builder,
                "Allure.TestingPlatform",
                () => new AllureTestingPlatformRuntimeRegistrationSession(),
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
