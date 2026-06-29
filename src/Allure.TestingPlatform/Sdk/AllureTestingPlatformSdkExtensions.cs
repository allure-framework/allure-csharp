using System;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform.Sdk;

public static class AllureTestingPlatformSdkExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        public IAllureTestingPlatformRuntimeRegistry AddEmbeddedAllure(
            Action<IEmbeddedAllureRegistrationContext> configureAllure
        ) =>
            AllureRegistrationFunctions.RegisterAllureTestingPlatform(
                builder,
                configureAllure,
                AllureTestingPlatformRegistrationMode.Embedded
            );

        public IAllureTestingPlatformRuntimeRegistry AddEmbeddedAllure() =>
            AddEmbeddedAllure(builder, static (_) => {});
    }

    extension (IEmbeddedAllureRegistrationContext context)
    {
        public IEmbeddedAllureRegistrationContext UseMtpSessionCorrelation() =>
            context.UseCorrelation((_, _) => new SessionUidCorrelation());

        public IEmbeddedAllureRegistrationContext UseTestNodeMetadataCorrelation() =>
            context.UseCorrelation((_, _) => new TestMetadataCorrelation());
    }
}
