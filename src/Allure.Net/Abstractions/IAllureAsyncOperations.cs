using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;

namespace Allure.Abstractions;

/// <summary>
/// Defines asynchronous operations exposed by an Allure API endpoint.
/// </summary>
/// <typeparam name="TStepContext">
/// The asynchronous step context type.
/// </typeparam>
/// <typeparam name="TFixtureContext">
/// The asynchronous fixture context type.
/// </typeparam>
public interface IAllureAsyncOperations<out TStepContext, out TFixtureContext>
    where TStepContext : IAllureAsyncStepContext
    where TFixtureContext : IAllureAsyncFixtureContext
{
    /// <summary>
    /// Asynchronously sets the name of the current test.
    /// </summary>
    Task SetTestNameAsync(string newName, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously sets the name of the current fixture.
    /// </summary>
    Task SetFixtureNameAsync(string newName, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously sets the name of the current step, fixture, or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the name change.
    /// </remarks>
    Task SetNameAsync(string newName, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously sets the Markdown description of the current test.
    /// </summary>
    Task SetDescriptionAsync(string description, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously sets the HTML description of the current test.
    /// </summary>
    Task SetDescriptionHtmlAsync(string descriptionHtml, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds labels to the current test.
    /// </summary>
    Task AddLabelsAsync(IEnumerable<Label> labels, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds a label to the current test.
    /// </summary>
    Task AddLabelAsync(Label label, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously sets a label, replacing labels with the same name.
    /// </summary>
    Task SetLabelAsync(string name, string value, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds links to the current test.
    /// </summary>
    Task AddLinksAsync(IEnumerable<Link> links, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds a link to the current test.
    /// </summary>
    Task AddLinkAsync(Link link, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds an attachment to the current step, fixture, or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the attachment.
    /// </remarks>
    Task AddAttachmentAsync(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously adds an attachment not associated with a test or fixture.
    /// </summary>
    Task AddGlobalAttachmentAsync(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously adds a screen diff attachment to the current step, fixture,
    /// or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the attachment.
    /// </remarks>
    Task AddScreenDiffAsync(
        Stream expected,
        Stream actual,
        Stream diff,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously adds a file attachment to the current step, fixture, or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the attachment.
    /// </remarks>
    Task AddFileAttachmentAsync(
        string name,
        string path,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously adds a file attachment not associated with a test or fixture.
    /// </summary>
    Task AddGlobalFileAttachmentAsync(
        string name,
        string path,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously adds a screen diff attachment from files to the current step,
    /// fixture, or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the attachment.
    /// </remarks>
    Task AddFileScreenDiffAsync(
        string expectedPath,
        string actualPath,
        string diffPath,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously adds an error not associated with a test or fixture.
    /// </summary>
    Task AddGlobalErrorAsync(GlobalError error, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds a parameter to the current test.
    /// </summary>
    Task AddTestParameterAsync(Parameter parameter, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds a completed step.
    /// </summary>
    Task StepAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Status status,
        StatusDetails? statusDetails,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs an asynchronous function as a step.
    /// </summary>
    Task StepAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a context-aware asynchronous function as a step.
    /// </summary>
    Task StepAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TStepContext, Task> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a step.
    /// </summary>
    Task StepAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TStepContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs an asynchronous function as a step and returns its result.
    /// </summary>
    Task<TResult> StepAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<TResult>> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a context-aware asynchronous function as a step and returns its result.
    /// </summary>
    Task<TResult> StepAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TStepContext, Task<TResult>> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a step and returns its result.
    /// </summary>
    Task<TResult> StepAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TStepContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs an asynchronous setup fixture.
    /// </summary>
    Task SetUpAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a context-aware asynchronous setup fixture.
    /// </summary>
    Task SetUpAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, Task> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous setup fixture.
    /// </summary>
    Task SetUpAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs an asynchronous setup fixture and returns its result.
    /// </summary>
    Task<TResult> SetUpAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<TResult>> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a context-aware asynchronous setup fixture and returns its result.
    /// </summary>
    Task<TResult> SetUpAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous setup fixture and returns its result.
    /// </summary>
    Task<TResult> SetUpAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs an asynchronous teardown fixture.
    /// </summary>
    Task TearDownAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a context-aware asynchronous teardown fixture.
    /// </summary>
    Task TearDownAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, Task> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous teardown fixture.
    /// </summary>
    Task TearDownAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs an asynchronous teardown fixture and returns its result.
    /// </summary>
    Task<TResult> TearDownAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<TResult>> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a context-aware asynchronous teardown fixture and returns its result.
    /// </summary>
    Task<TResult> TearDownAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous teardown fixture and returns its result.
    /// </summary>
    Task<TResult> TearDownAsync<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    );
}
