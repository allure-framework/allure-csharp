using System;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Builder;
using Allure.TestingPlatform.Internal.Registration;
using Microsoft.Testing.Platform.Extensions;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;

namespace Allure.TestingPlatform.Sdk;

public static class AllureTestingPlatformSdkExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        internal IAllureTestingPlatformRuntimeReferenceRegistry RegisterAllureTestingPlatform(
            Action<AllureTestingPlatformRegistration> configureAllure,
            AllureTestingPlatformRegistrationMode registrationMode
        )
        {
            builder.CommandLine.AddProvider(() => new AllureCliOptionsProvider());

            var allureRegistration = new AllureTestingPlatformRegistration(registrationMode);
            configureAllure(allureRegistration);
            var frozenRegistration = allureRegistration.Prepare();
            var allureRuntimeReferences = frozenRegistration.RuntimeReferences;

            var factory =
                new CompositeExtensionFactory<AllureDataConsumer>((serviceProvider) =>
                    new AllureDataConsumer(
                        allureRuntimeReferences.GetRuntimeReference(serviceProvider)
                    )
                );

            if (frozenRegistration.HostProcessWatchdogEnabled)
            {
                builder.TestHostControllers.AddProcessLifetimeHandler((serviceProvider) =>
                    new AllureTestingPlatformHostProcessWatchdog(
                        frozenRegistration.CreateController(serviceProvider)
                    )
                );
            }

            builder.TestHost.AddTestHostApplicationLifetime((serviceProvider) =>
                new AllureTestingPlatformInProcessRuntimeController(
                    frozenRegistration.CreateController(serviceProvider)
                )
            );
            builder.TestHost.AddDataConsumer(factory);
            builder.TestHost.AddTestSessionLifetimeHandler(factory);

            return frozenRegistration.RuntimeReferences;
        }

        public IAllureTestingPlatformRuntimeReferenceRegistry AddEmbeddedAllure(
            Action<IEmbeddedAllureRegistrationContext> configureAllure
        ) =>
            RegisterAllureTestingPlatform(
                builder,
                configureAllure,
                AllureTestingPlatformRegistrationMode.Embedded
            );

        public IAllureTestingPlatformRuntimeReferenceRegistry AddEmbeddedAllure() =>
            AddEmbeddedAllure(builder, static (_) => {});
    }

    extension (IEmbeddedAllureRegistrationContext context)
    {
        public IEmbeddedAllureRegistrationContext UseMtpSessionCorrelation() =>
            context.UseCorrelation((_, _) => new TestingPlatformSessionUidCorrelationStrategy());

        public IEmbeddedAllureRegistrationContext UseTestNodeMetadataCorrelation() =>
            context.UseCorrelation((_, _) => new TestNodeMetadataCorrelationStrategy());
    }
}
