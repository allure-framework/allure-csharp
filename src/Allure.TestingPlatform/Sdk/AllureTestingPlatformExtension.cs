using System;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Messages;

namespace Allure.TestingPlatform.Sdk;

/// <summary>
/// Base class for Allure.TestingPlatform extensions.
/// </summary>
public abstract class AllureTestingPlatformExtension<TConfiguration, TRuntime> : IExtension
    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    readonly IReadOnlyLateBoundReference<TRuntime> runtimeReference;

    /// <inheritdoc />
    public string Uid { get; }

    /// <inheritdoc />
    public string Version { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public string Description { get; }

    protected TRuntime Runtime => this.runtimeReference.Value;

    /// <summary>
    /// Gets the resolved Allure configuration.
    /// </summary>
    protected TConfiguration Configuration => this.Runtime.Configuration;

    /// <summary>
    /// Gets the resolved Allure context.
    /// </summary>
    protected IAllureExecutionContext ContextApi => this.Runtime.ContextApi;

    /// <summary>
    /// Gets the resolved correlation context.
    /// </summary>
    protected ICorrelationContext CorrelationContext => this.Runtime.CorrelationContext;

    /// <summary>
    /// Gets the resolved correlation strategy.
    /// </summary>
    protected ICorrelationStrategy CorrelationStrategy => this.Runtime.CorrelationStrategy;

    /// <summary>
    /// Gets the resolved execution state context.
    /// </summary>
    protected ExecutionStateContext ExecutionStateContext => this.Runtime.ExecutionStateContext;

    /// <summary>
    /// Gets the resolved Allure lifecycle API.
    /// </summary>
    protected IAllureLifecycleApi LifecycleApi => this.Runtime.LifecycleApi;

    /// <summary>
    /// Gets the configured runtime logger.
    /// </summary>
    protected ILogger Logger => this.Runtime.Logger;

    /// <summary>
    /// Gets the Mictosoft Testing Platform message bus.
    /// </summary>
    protected IMessageBus MessageBus => this.Runtime.MessageBus;

    /// <summary>
    /// Gets the resolved Allure object model API.
    /// </summary>
    protected IAllureModelApi ModelApi => this.Runtime.ModelApi;

    /// <summary>
    /// Gets the resolved parameter serializer.
    /// </summary>
    protected IAllureParameterSerializer ParameterSerializer => this.Runtime.ParameterSerializer;

    /// <summary>
    /// Gets the resolved Allure results destination.
    /// </summary>
    protected IAllureResultsDestination ResultsDestination => this.Runtime.ResultsDestination;

    public AllureTestingPlatformExtension(
        string uid,
        string displayName,
        string description,
        IReadOnlyLateBoundReference<TRuntime> runtimeReference
    )
    {
        this.Uid = uid;
        this.DisplayName = displayName;
        this.Description = description;
        this.Version = TestingPlatformFunctions.GetPackageVersion(this.GetType());
        this.runtimeReference = runtimeReference;
    }

    /// <inheritdoc />
    public virtual Task<bool> IsEnabledAsync() =>
        this.runtimeReference.IsBound
            ? Task.FromResult(this.Configuration.IsEnabled)
            : throw new InvalidOperationException(
                "Unexpected error: The Allure.TestingPlatform runtime is not registered."
            );
}
