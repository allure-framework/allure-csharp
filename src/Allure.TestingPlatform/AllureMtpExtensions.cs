using System;
using System.Threading;
using Allure.TestingPlatform.Registration;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.Services;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform;

public static class AllureMtpExtensions
{
    static IMessageBus? messageBus = null;
    static IAllureInfrastructure? allure = null;

    extension (ITestApplicationBuilder builder)
    {
        public void AddAllure(Action<IAllureRegistrationContext> allureRegistration)
        {
            var allureBuilder = new AllureInfrastructureBuilder();

            builder.CommandLine.AddProvider(() => new AllureCliOptionsProvider());

            allureRegistration(allureBuilder);

            builder.TestHost.AddDataConsumer((serviceProvider) =>
            {
                messageBus = serviceProvider.GetMessageBus();
                allure = allureBuilder.Build(serviceProvider);
                return new AllureDataConsumer(allure);
            });
        }

        public void AddAllure() =>
            AddAllure(builder, static (_) => {});
    }

    extension (IAllureInfrastructure)
    {
        public static IMessageBus MessageBus =>
            messageBus ?? throw new InvalidOperationException("Allure is not initialized");

        public static IAllureInfrastructure Allure =>
            allure ?? throw new InvalidOperationException("Allure is not initialized");
    }

    static readonly AsyncLocal<SessionUid?> currentSessionUid = new();

    extension (SessionUid)
    {
        public static SessionUid? Current
        {
            get => currentSessionUid.Value;
            internal set => currentSessionUid.Value = value;
        }
    }
}
