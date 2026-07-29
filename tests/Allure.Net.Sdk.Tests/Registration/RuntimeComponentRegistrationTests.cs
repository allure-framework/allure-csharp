using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.Registration;

public class RuntimeComponentRegistrationTests
{
    [Test]
    public async Task ShouldCreateDefaultDestinationFromConfiguration()
    {
        var directory = NewDirectoryPath();
        try
        {
            var configuration = new TestConfiguration
            {
                ResultsDirectory = directory,
                IndentOutput = true,
            };

            var destination =
                AllureRegistrationDefaults.Destination<TestConfiguration>()(
                    configuration
                );

            await Assert.That(destination)
                .IsTypeOf<FileSystemResultsDestination>();

            destination.WriteGlobals(new()
            {
                Errors = { new() { Message = "factory error" } },
            });

            var json = await File.ReadAllTextAsync(
                Directory.GetFiles(directory, "*-globals.json").Single()
            );
            await Assert.That(json).Contains("factory error");
            await Assert.That(json.Contains('\n')).IsTrue();
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldUseFinalConfigurationForDefaultDestinationInBuiltRuntime()
    {
        var originalDirectory = NewDirectoryPath();
        var finalDirectory = NewDirectoryPath();
        try
        {
            var original = new TestConfiguration
            {
                ResultsDirectory = originalDirectory,
            };
            var final = new TestConfiguration
            {
                ResultsDirectory = finalDirectory,
                IndentOutput = true,
            };
            var builder = CreateBuilder();
            builder.UseConfiguration(original);
            builder.UseRegistrationHooks(_ =>
            [
                new RecordingRuntimeHook<TestConfiguration>(
                    context => context.UseConfiguration(final)
                ),
            ]);

            var runtime = builder.Build();
            runtime.ResultsDestination.WriteGlobals(new()
            {
                Errors = { new() { Message = "runtime error" } },
            });

            await Assert.That(runtime.ResultsDestination)
                .IsTypeOf<FileSystemResultsDestination>();
            await Assert.That(runtime.Configuration).IsSameReferenceAs(final);
            await Assert.That(Directory.Exists(originalDirectory)).IsFalse();
            var json = await File.ReadAllTextAsync(
                Directory.GetFiles(finalDirectory, "*-globals.json").Single()
            );
            await Assert.That(json).Contains("runtime error");
            await Assert.That(json.Contains('\n')).IsTrue();
        }
        finally
        {
            DeleteDirectory(originalDirectory);
            DeleteDirectory(finalDirectory);
        }
    }

    [Test]
    public async Task ShouldBuildRuntimeFromFinalConfigurationAndRegisteredComponents()
    {
        var configuration = new TestConfiguration();
        var serializer = new RecordingParameterSerializer(_ => "serialized");
        var destination = new InMemoryResultsDestination();

        var context = IAllureExecutionContext.Mock();
        var lifecycle = IAllureLifecycleApi.Mock();
        var model = IAllureModelApi.Mock();

        var dependencyInputs =
            new List<IAllureRegistrationDependencies<TestConfiguration>>();

        TestConfiguration? serializerConfiguration = null;
        TestConfiguration? destinationConfiguration = null;

        var builder = CreateBuilder();

        builder.UseConfiguration(configuration);
        builder.UseParameterSerializer(received =>
        {
            serializerConfiguration = received;
            return serializer;
        });
        builder.UseDestination(received =>
        {
            destinationConfiguration = received;
            return destination;
        });
        builder.UseContext(dependencies =>
        {
            dependencyInputs.Add(dependencies);
            return context;
        });
        builder.UseLifecycleApi(dependencies =>
        {
            dependencyInputs.Add(dependencies);
            return lifecycle;
        });
        builder.UseModelApi(dependencies =>
        {
            dependencyInputs.Add(dependencies);
            return model;
        });

        var runtime = builder.Build();

        await Assert.That(serializerConfiguration)
            .IsSameReferenceAs(configuration);
        await Assert.That(destinationConfiguration)
            .IsSameReferenceAs(configuration);
        await Assert.That(dependencyInputs.Count).IsEqualTo(3);
        await Assert.That(dependencyInputs.Distinct().Count()).IsEqualTo(1);
        await Assert.That(dependencyInputs[0].Configuration)
            .IsSameReferenceAs(configuration);
        await Assert.That(dependencyInputs[0].ParameterSerializer)
            .IsSameReferenceAs(serializer);
        await Assert.That(runtime.Configuration).IsSameReferenceAs(configuration);
        await Assert.That(runtime.ParameterSerializer).IsSameReferenceAs(serializer);
        await Assert.That(runtime.ResultsDestination).IsSameReferenceAs(destination);
        await Assert.That(runtime.ContextApi).IsSameReferenceAs(context);
        await Assert.That(runtime.LifecycleApi).IsSameReferenceAs(lifecycle);
        await Assert.That(runtime.ModelApi).IsSameReferenceAs(model);
        await Assert.That(dependencyInputs[0].RuntimeReference.Value)
            .IsSameReferenceAs(runtime);
    }

    [Test]
    public async Task ShouldThrowWhenRuntimeReferenceIsReadDirectlyFromFactory()
    {
        var builder = CreateBuilder();
        builder.UseConfiguration(new TestConfiguration());
        builder.UseContext(dependencies =>
        {
            _ = dependencies.RuntimeReference.Value;
            return IAllureExecutionContext.Mock();
        });

        await Assert.That(builder.Build)
            .Throws<InvalidOperationException>()
            .WithMessageContaining("has not been bound");
    }

    static AllureRuntimeBuilder<
        TestConfiguration,
        RecordingRuntimeHook<TestConfiguration>,
        RecordingEndpointHook<TestConfiguration>
    > CreateBuilder() =>
        new("component-tests");

    static string NewDirectoryPath() =>
        Path.Combine(Path.GetTempPath(), $"allure-sdk-registration-{Guid.NewGuid():N}");

    static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    sealed record class TestConfiguration : AllureConfiguration;
}
