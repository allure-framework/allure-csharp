using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;

namespace Allure.Abstractions;

public interface IAllureAsyncOperations<out TStepContext, out TFixtureContext>
    where TStepContext : IAllureAsyncStepContext
    where TFixtureContext : IAllureAsyncFixtureContext
{
    Task SetTestNameAsync(string newName, CancellationToken cancellationToken);

    Task SetFixtureNameAsync(string newName, CancellationToken cancellationToken);

    Task SetNameAsync(string newName, CancellationToken cancellationToken);

    Task SetDescriptionAsync(string description, CancellationToken cancellationToken);

    Task SetDescriptionHtmlAsync(string descriptionHtml, CancellationToken cancellationToken);

    Task AddLabelsAsync(IEnumerable<Label> labels, CancellationToken cancellationToken);

    Task AddLabelAsync(Label label, CancellationToken cancellationToken);

    Task SetLabelAsync(string name, string value, CancellationToken cancellationToken);

    Task AddLinksAsync(IEnumerable<Link> links, CancellationToken cancellationToken);

    Task AddLinkAsync(Link link, CancellationToken cancellationToken);

    Task AddAttachmentAsync(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken);

    Task AddGlobalAttachmentAsync(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken);

    Task AddScreenDiffAsync(Stream expected, Stream actual, Stream diff, CancellationToken cancellationToken);

    Task AddFileAttachmentAsync(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken);

    Task AddGlobalFileAttachmentAsync(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken);

    Task AddFileScreenDiffAsync(string expectedPath, string actualPath, string diffPath, CancellationToken cancellationToken);

    Task AddGlobalErrorAsync(GlobalError error, CancellationToken cancellationToken);

    Task AddTestParameterAsync(Parameter parameter, CancellationToken cancellationToken);

    Task StepAsync(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails, CancellationToken cancellationToken);

    Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken);

    Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<TStepContext, Task> body, CancellationToken cancellationToken);

    Task StepAsync(string name, IEnumerable<Parameter> parameters, Func<TStepContext, CancellationToken, Task> body, CancellationToken cancellationToken);

    Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<TStepContext, Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> StepAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<TStepContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken);

    Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken);

    Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, Task> body, CancellationToken cancellationToken);

    Task SetUpAsync(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken);

    Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> SetUpAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken);

    Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken);

    Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, Task> body, CancellationToken cancellationToken);

    Task TearDownAsync(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken);

    Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> TearDownAsync<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken);
}
