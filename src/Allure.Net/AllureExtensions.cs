using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Runtime;

namespace Allure;

public static class AllureExtensions
{
    extension (IAllureStepContext stepContext)
    {
        public void AddParameter(string name, string value) =>
            stepContext.AddParameter(new()
            {
                Name = name,
                Value = value,
            });

        public void AddParameter(string name, string value, ParameterMode mode) =>
            stepContext.AddParameter(new()
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
                stepContext,
                name,
                AllureFrontend.Client.ParameterSerializer.Serialize(value)
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
                stepContext,
                name,
                AllureFrontend.Client.ParameterSerializer.Serialize(value),
                mode
            );
    }

    extension (IAllureAsyncStepContext stepContext)
    {
        /// <summary>
        /// Sets the name of the step associated with this context.
        /// </summary>
        /// <param name="newName">The new name of the step.</param>
        public Task SetName(string newName) =>
            stepContext.SetNameAsync(newName, default);

        /// <summary>
        /// Adds a fully constructed parameter to the step.
        /// </summary>
        /// <param name="parameter">A parameter to add.</param>
        public Task AddParameter(Parameter parameter) =>
            stepContext.AddParameterAsync(parameter, default);

        public Task AddParameter(string name, string value) =>
            stepContext.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, default);

        public Task AddParameter(string name, string value, CancellationToken cancellationToken) =>
            stepContext.AddParameterAsync(new()
            {
                Name = name,
                Value = value,
            }, cancellationToken);

        public void AddParameter(
            string name,
            string value,
            ParameterMode mode
        ) =>
            stepContext.AddParameterAsync(new()
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
            stepContext.AddParameterAsync(new()
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
                stepContext,
                name,
                AllureFrontend.Client.ParameterSerializer.Serialize(value),
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
                stepContext,
                name,
                AllureFrontend.Client.ParameterSerializer.Serialize(value),
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
                stepContext,
                name,
                AllureFrontend.Client.ParameterSerializer.Serialize(value),
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
                stepContext,
                name,
                AllureFrontend.Client.ParameterSerializer.Serialize(value),
                mode,
                cancellationToken
            );
    }
}