using System;
using Allure.TestingPlatform.Registration;
using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform;

public static class AllureTestingPlatformExtensions
{
    public static void AddAllure(
        this ITestApplicationBuilder builder,
        Action<IAllureRegistrationContext> allureRegistration
    )
    {
        var allureBuilder = new AllureInfrastructureBuilder();
        allureRegistration(allureBuilder);

        builder.TestHost.AddDataConsumer((serviceProvider) =>
            new AllureDataConsumer(
                allureBuilder.Build()
            ));
    }

    public static void RegisterAllure(this ITestApplicationBuilder builder) =>
        AddAllure(builder, static (_) => {});
}