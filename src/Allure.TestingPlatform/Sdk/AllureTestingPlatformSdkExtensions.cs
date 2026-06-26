using System;
using System.Collections.Immutable;
using Allure.TestingPlatform.Internal;
using Allure.TestingPlatform.Registration;

namespace Allure.TestingPlatform.Sdk;

public static class AllureTestingPlatformSdkExtensions
{
    extension (IAllureRegistrationContext context)
    {
        public IAllureRegistrationContext UseMtpSessionCorrelation() =>
            context.UseCorrelationService((_, _) => new SessionUidCorrelation());

        public IAllureRegistrationContext UseTestNodeMetadataCorrelation() =>
            context.UseCorrelationService((_, _) => new TestMetadataCorrelation());
    }

    static ImmutableDictionary<IServiceProvider, IAllureExtensionSettings> settingsByServiceProvider =
        ImmutableDictionary.CreateBuilder<IServiceProvider, IAllureExtensionSettings>(
            ReferenceEqualityComparer<IServiceProvider>.Instance
        ).ToImmutable();

    extension (IServiceProvider serviceProvider)
    {
        public IAllureExtensionSettings AllureExtensionSettings
        {
            get => settingsByServiceProvider.TryGetValue(serviceProvider, out var settings)
                ? settings
                : throw new InvalidOperationException(
                    $"Internal Allure error: cannot get extension settings."
                );

            internal set
            {
                lock (settingsByServiceProvider)
                {
                    settingsByServiceProvider =
                        settingsByServiceProvider.TryGetValue(serviceProvider, out var _) is false
                            ? settingsByServiceProvider.Add(serviceProvider, value)
                            : throw new InvalidOperationException(
                                $"Internal Allure error: extension settings already defined."
                            );
                }
            }
        }
    }
}
