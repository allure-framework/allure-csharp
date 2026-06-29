using System;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform.Sdk;

public static class AllureTestingPlatformSdkExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        public IAllureTestingPlatformRegistrationResult AddEmbeddedAllure(
            Action<IEmbeddedRegistrationContext> configureAllure
        ) =>
            AllureRegistrationFunctions.RegisterAllureTestingPlatform(
                builder,
                configureAllure,
                AllureTestingPlatformRegistrationMode.Embedded
            );

        public IAllureTestingPlatformRegistrationResult AddEmbeddedAllure() =>
            AddEmbeddedAllure(builder, static (_) => {});
    }

    extension (IEmbeddedRegistrationContext context)
    {
        public IEmbeddedRegistrationContext UseMtpSessionCorrelation() =>
            context.UseCorrelation((_, _) => new SessionUidCorrelation());

        public IEmbeddedRegistrationContext UseTestNodeMetadataCorrelation() =>
            context.UseCorrelation((_, _) => new TestMetadataCorrelation());
    }
}
