using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Provides convenience operations for step and fixture contexts.
/// </summary>
public static class AllureContextExtensions
{
    extension (IAllureSyncStepContext context)
    {
        /// <summary>
        /// Adds a parameter to the current step.
        /// </summary>
        public void AddParameter(string name, string value) =>
            context.AddParameter(new()
            {
                Name = name,
                Value = value,
            });

        /// <summary>
        /// Adds a parameter with the specified display mode to the current step.
        /// </summary>
        public void AddParameter(string name, string value, ParameterMode mode) =>
            context.AddParameter(new()
            {
                Name = name,
                Value = value,
                Mode = mode,
            });

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to serialize.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(string name, object? value) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value)
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="mode">The parameter display mode.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(string name, object? value, ParameterMode mode) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                mode
            );
    }

    extension (IAllureAsyncStepContext context)
    {
        /// <summary>
        /// Sets the name of the step associated with this context.
        /// </summary>
        /// <param name="newName">The new name of the step.</param>
        public Task SetNameAsync(string newName) =>
            context.SetNameAsync(newName, default);

        /// <summary>
        /// Adds a fully constructed parameter to the step.
        /// </summary>
        /// <param name="parameter">A parameter to add.</param>
        public Task AddParameterAsync(Parameter parameter) =>
            context.AddParameterAsync(parameter, default);

        /// <summary>
        /// Asynchronously adds a parameter to the current step.
        /// </summary>
        public Task AddParameterAsync(string name, string value) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, default);

        /// <summary>
        /// Asynchronously adds a parameter to the current step.
        /// </summary>
        public Task AddParameterAsync(string name, string value, CancellationToken cancellationToken) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, cancellationToken);

        /// <summary>
        /// Asynchronously adds a parameter with the specified display mode to the current step.
        /// </summary>
        public Task AddParameterAsync(
            string name,
            string value,
            ParameterMode mode
        ) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
                Mode = mode,
            }, default);

        /// <summary>
        /// Asynchronously adds a parameter with the specified display mode to the current step.
        /// </summary>
        public Task AddParameterAsync(
            string name,
            string value,
            ParameterMode mode,
            CancellationToken cancellationToken
        ) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
                Mode = mode,
            }, cancellationToken);

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public Task AddParameterFromObjectAsync(
            string name,
            object? value
        ) =>
            AddParameterAsync(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                CancellationToken.None
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public Task AddParameterFromObjectAsync(
            string name,
            object? value,
            CancellationToken cancellationToken
        ) =>
            AddParameterAsync(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                cancellationToken
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="mode">The parameter display mode.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public Task AddParameterFromObjectAsync(
            string name,
            object value,
            ParameterMode mode
        ) =>
            AddParameterAsync(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                mode,
                default
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="mode">The parameter display mode.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public Task AddParameterFromObjectAsync(
            string name,
            object? value,
            ParameterMode mode,
            CancellationToken cancellationToken
        ) =>
            AddParameterAsync(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                mode,
                cancellationToken
            );
    }

    extension (IAllureSyncFixtureContext context)
    {
        /// <summary>
        /// Adds a parameter to the current fixture.
        /// </summary>
        public void AddParameter(string name, string value) =>
            context.AddParameter(new()
            {
                Name = name,
                Value = value,
            });

        /// <summary>
        /// Adds a parameter with the specified display mode to the current fixture.
        /// </summary>
        public void AddParameter(string name, string value, ParameterMode mode) =>
            context.AddParameter(new()
            {
                Name = name,
                Value = value,
                Mode = mode,
            });

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to serialize.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(string name, object? value) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value)
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="mode">The parameter display mode.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(string name, object? value, ParameterMode mode) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                mode
            );
    }

    extension (IAllureAsyncFixtureContext context)
    {
        /// <summary>
        /// Sets the name of the fixture associated with this context.
        /// </summary>
        /// <param name="newName">The new name of the fixture.</param>
        public Task SetNameAsync(string newName) =>
            context.SetNameAsync(newName, default);

        /// <summary>
        /// Adds a fully constructed parameter to the fixture.
        /// </summary>
        /// <param name="parameter">A parameter to add.</param>
        public Task AddParameterAsync(Parameter parameter) =>
            context.AddParameterAsync(parameter, default);

        /// <summary>
        /// Asynchronously adds a parameter to the current fixture.
        /// </summary>
        public Task AddParameterAsync(string name, string value) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, default);

        /// <summary>
        /// Asynchronously adds a parameter to the current fixture.
        /// </summary>
        public Task AddParameterAsync(string name, string value, CancellationToken cancellationToken) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, cancellationToken);

        /// <summary>
        /// Asynchronously adds a parameter with the specified display mode to the current fixture.
        /// </summary>
        public Task AddParameterAsync(
            string name,
            string value,
            ParameterMode mode
        ) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
                Mode = mode,
            }, default);

        /// <summary>
        /// Asynchronously adds a parameter with the specified display mode to the current fixture.
        /// </summary>
        public Task AddParameterAsync(
            string name,
            string value,
            ParameterMode mode,
            CancellationToken cancellationToken
        ) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
                Mode = mode,
            }, cancellationToken);

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public Task AddParameterFromObjectAsync(
            string name,
            object? value
        ) =>
            AddParameterAsync(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                CancellationToken.None
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public Task AddParameterFromObjectAsync(
            string name,
            object? value,
            CancellationToken cancellationToken
        ) =>
            AddParameterAsync(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                cancellationToken
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="mode">The parameter display mode.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public Task AddParameterFromObjectAsync(
            string name,
            object value,
            ParameterMode mode
        ) =>
            AddParameterAsync(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                mode,
                default
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="mode">The parameter display mode.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public Task AddParameterFromObjectAsync(
            string name,
            object? value,
            ParameterMode mode,
            CancellationToken cancellationToken
        ) =>
            AddParameterAsync(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                mode,
                cancellationToken
            );
    }

    extension (IAllureInProcessSyncStepContext context)
    {
        /// <summary>
        /// Reads a value from the step result associated with this context.
        /// If the step is inaccessible, throws an exception.
        /// </summary>
        public TResult ReadStepResult<TResult>(
            Func<StepResult, TResult> read
        ) =>
            context.TryReadStepResult(read, out var result)
                ? result
                : throw new InvalidOperationException(
                    "Cannot read step result: the step associated with this context is not running."
                );

        /// <summary>
        /// Reads a value from the step result associated with this context, or returns a fallback value.
        /// </summary>
        public TResult ReadStepResult<TResult>(
            Func<StepResult, TResult> read,
            TResult fallback
        ) =>
            context.TryReadStepResult(read, out var result)
                ? result
                : fallback;

        /// <summary>
        /// Reads a value from the step result associated with this context, or creates a fallback value.
        /// </summary>
        public TResult ReadStepResult<TResult>(
            Func<StepResult, TResult> read,
            Func<TResult> fallbackFactory
        ) =>
            context.TryReadStepResult(read, out var result)
                ? result
                : fallbackFactory();
    }

    extension (IAllureInProcessAsyncStepContext context)
    {
        /// <summary>
        /// Reads a value from the step result associated with this context.
        /// If the step is inaccessible, throws an exception.
        /// </summary>
        public TResult ReadStepResult<TResult>(
            Func<StepResult, TResult> read
        ) =>
            context.TryReadStepResult(read, out var result)
                ? result
                : throw new InvalidOperationException(
                    "Cannot read step result: the step associated with this context is not running."
                );

        /// <summary>
        /// Reads a value from the step result associated with this context, or returns a fallback value.
        /// </summary>
        public TResult ReadStepResult<TResult>(
            Func<StepResult, TResult> read,
            TResult fallback
        ) =>
            context.TryReadStepResult(read, out var result)
                ? result
                : fallback;

        /// <summary>
        /// Reads a value from the step result associated with this context, or creates a fallback value.
        /// </summary>
        public TResult ReadStepResult<TResult>(
            Func<StepResult, TResult> read,
            Func<TResult> fallbackFactory
        ) =>
            context.TryReadStepResult(read, out var result)
                ? result
                : fallbackFactory();
    }

    extension (IAllureInProcessSyncFixtureContext context)
    {
        /// <summary>
        /// Reads a value from the fixture result associated with this context.
        /// If the fixture is inaccessible, throws an exception.
        /// </summary>
        public TResult ReadFixtureResult<TResult>(
            Func<FixtureResult, TResult> read
        ) =>
            context.TryReadFixtureResult(read, out var result)
                ? result
                : throw new InvalidOperationException(
                    "Cannot read fixture result: the fixture associated with this context is not running."
                );

        /// <summary>
        /// Reads a value from the fixture result associated with this context, or returns a fallback value.
        /// </summary>
        public TResult ReadFixtureResult<TResult>(
            Func<FixtureResult, TResult> read,
            TResult fallback
        ) =>
            context.TryReadFixtureResult(read, out var result)
                ? result
                : fallback;

        /// <summary>
        /// Reads a value from the fixture result associated with this context, or creates a fallback value.
        /// </summary>
        public TResult ReadFixtureResult<TResult>(
            Func<FixtureResult, TResult> read,
            Func<TResult> fallbackFactory
        ) =>
            context.TryReadFixtureResult(read, out var result)
                ? result
                : fallbackFactory();
    }

    extension (IAllureInProcessAsyncFixtureContext context)
    {
        /// <summary>
        /// Reads a value from the fixture result associated with this context.
        /// If the fixture is inaccessible, throws an exception.
        /// </summary>
        public TResult ReadFixtureResult<TResult>(
            Func<FixtureResult, TResult> read
        ) =>
            context.TryReadFixtureResult(read, out var result)
                ? result
                : throw new InvalidOperationException(
                    "Cannot read fixture result: the fixture associated with this context is not running."
                );

        /// <summary>
        /// Reads a value from the fixture result associated with this context, or returns a fallback value.
        /// </summary>
        public TResult ReadFixtureResult<TResult>(
            Func<FixtureResult, TResult> read,
            TResult fallback
        ) =>
            context.TryReadFixtureResult(read, out var result)
                ? result
                : fallback;

        /// <summary>
        /// Reads a value from the fixture result associated with this context, or creates a fallback value.
        /// </summary>
        public TResult ReadFixtureResult<TResult>(
            Func<FixtureResult, TResult> read,
            Func<TResult> fallbackFactory
        ) =>
            context.TryReadFixtureResult(read, out var result)
                ? result
                : fallbackFactory();
    }
}
