using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

public static class AllureContextExtensions
{
    extension (IAllureStepContext context)
    {
        public void AddParameter(string name, string value) =>
            context.AddParameter(new()
            {
                Name = name,
                Value = value,
            });

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
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <param name="mode">A display mode of the parameter.</param>
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
        public Task SetName(string newName) =>
            context.SetNameAsync(newName, default);

        /// <summary>
        /// Adds a fully constructed parameter to the step.
        /// </summary>
        /// <param name="parameter">A parameter to add.</param>
        public Task AddParameter(Parameter parameter) =>
            context.AddParameterAsync(parameter, default);

        public Task AddParameter(string name, string value) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, default);

        public Task AddParameter(string name, string value, CancellationToken cancellationToken) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, cancellationToken);

        public void AddParameter(
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

        public void AddParameter(
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
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(
            string name,
            object? value
        ) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                CancellationToken.None
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(
            string name,
            object? value,
            CancellationToken cancellationToken
        ) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                cancellationToken
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <param name="mode">A display mode of the parameter.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(
            string name,
            object value,
            ParameterMode mode
        ) =>
            AddParameter(
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
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <param name="mode">A display mode of the parameter.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(
            string name,
            object? value,
            ParameterMode mode,
            CancellationToken cancellationToken
        ) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                mode,
                cancellationToken
            );
    }

    extension (IAllureFixtureContext context)
    {
        public void AddParameter(string name, string value) =>
            context.AddParameter(new()
            {
                Name = name,
                Value = value,
            });

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
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <param name="mode">A display mode of the parameter.</param>
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
        /// <param name="newName">The new name of the step.</param>
        public Task SetName(string newName) =>
            context.SetNameAsync(newName, default);

        /// <summary>
        /// Adds a fully constructed parameter to the step.
        /// </summary>
        /// <param name="parameter">A parameter to add.</param>
        public Task AddParameter(Parameter parameter) =>
            context.AddParameterAsync(parameter, default);

        public Task AddParameter(string name, string value) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, default);

        public Task AddParameter(string name, string value, CancellationToken cancellationToken) =>
            context.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, cancellationToken);

        public void AddParameter(
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

        public void AddParameter(
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
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(
            string name,
            object? value
        ) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                CancellationToken.None
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(
            string name,
            object? value,
            CancellationToken cancellationToken
        ) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                cancellationToken
            );

        /// <summary>
        /// Adds a parameter whose value is obtained by serializing the specified
        /// CLR value.
        /// </summary>
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <param name="mode">A display mode of the parameter.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(
            string name,
            object value,
            ParameterMode mode
        ) =>
            AddParameter(
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
        /// <param name="name">A name of the parameter.</param>
        /// <param name="value">A value to serialize.</param>
        /// <param name="mode">A display mode of the parameter.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <remarks>
        /// The value is converted to text using the parameter serializer
        /// configured for the current Allure runtime.
        /// </remarks>
        public void AddParameterFromObject(
            string name,
            object? value,
            ParameterMode mode,
            CancellationToken cancellationToken
        ) =>
            AddParameter(
                context,
                name,
                context.ParameterSerializer.Serialize(value),
                mode,
                cancellationToken
            );
    }
}