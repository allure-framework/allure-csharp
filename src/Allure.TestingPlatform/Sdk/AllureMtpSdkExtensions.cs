using Allure.TestingPlatform.Registration;

namespace Allure.TestingPlatform.Sdk;

public static class AllureMtpSdkExtensions
{
    extension (IAllureRegistrationContext context)
    {
        public IAllureRegistrationContext UseMtpSessionCorrelation() =>
            context.UseCorrelationService((_, _) => new SessionUidCorrelation());

        public IAllureRegistrationContext UseTestNodeMetadataCorrelation() =>
            context.UseCorrelationService((_, _) => new TestMetadataCorrelation());
    }
}
