using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;

namespace Allure.Abstractions;

public interface IAllureTestApiAsync<out TStepContext, out TFixtureContext>
    where TStepContext : IAllureStepContextAsync
    where TFixtureContext : IAllureFixtureContextAsync
{
    Task SetTestName(string newName, CancellationToken cancellationToken);

    Task SetFixtureName(string newName, CancellationToken cancellationToken);

    Task SetName(string newName, CancellationToken cancellationToken);

    Task SetDescription(string description, CancellationToken cancellationToken);

    Task SetDescriptionHtml(string descriptionHtml, CancellationToken cancellationToken);

    Task AddLabels(IEnumerable<Label> labels, CancellationToken cancellationToken);

    Task AddLabel(Label label, CancellationToken cancellationToken);

    Task SetLabel(string name, string value, CancellationToken cancellationToken);

    Task AddLinks(IEnumerable<Link> links, CancellationToken cancellationToken);

    Task AddLink(Link link, CancellationToken cancellationToken);

    Task AddAttachment(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken);

    Task AddGlobalAttachment(string name, Stream content, string? mediaType, string fileExtension, CancellationToken cancellationToken);

    Task AddScreenDiff(Stream expected, Stream actual, Stream diff, CancellationToken cancellationToken);

    Task AddFileAttachment(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken);

    Task AddGlobalFileAttachment(string name, string path, string? mediaType, string fileExtension, CancellationToken cancellationToken);

    Task AddFileScreenDiff(string expectedPath, string actualPath, string diffPath, CancellationToken cancellationToken);

    Task AddGlobalError(GlobalError error, CancellationToken cancellationToken);

    Task AddTestParameter(Parameter parameter, CancellationToken cancellationToken);

    Task Step(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails, CancellationToken cancellationToken);

    Task Step(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken);

    Task Step(string name, IEnumerable<Parameter> parameters, Func<TStepContext, Task> body, CancellationToken cancellationToken);

    Task Step(string name, IEnumerable<Parameter> parameters, Func<TStepContext, CancellationToken, Task> body, CancellationToken cancellationToken);

    Task<TResult> Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<TStepContext, Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<TStepContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken);

    Task SetUp(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken);

    Task SetUp(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, Task> body, CancellationToken cancellationToken);

    Task SetUp(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken);

    Task<TResult> SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken);

    Task TearDown(string name, IEnumerable<Parameter> parameters, Func<Task> body, CancellationToken cancellationToken);

    Task TearDown(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, Task> body, CancellationToken cancellationToken);

    Task TearDown(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, CancellationToken, Task> body, CancellationToken cancellationToken);

    Task<TResult> TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, Task<TResult>> body, CancellationToken cancellationToken);

    Task<TResult> TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, CancellationToken, Task<TResult>> body, CancellationToken cancellationToken);
}
