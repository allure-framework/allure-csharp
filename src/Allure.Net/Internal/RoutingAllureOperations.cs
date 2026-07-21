using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Allure.Abstractions;
using Allure.Model;
using Allure.Runtime;

namespace Allure.Internal;

class RoutingAllureOperations : IAllureInProcessOperations
{
    public void AddAttachment(string name, Stream content, string? mediaType, string fileExtension) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.AddAttachment(name, content, mediaType, fileExtension);

    public void AddFileAttachment(string name, string path, string? mediaType, string fileExtension) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.AddFileAttachment(name, path, mediaType, fileExtension);

    public void AddFileScreenDiff(string expectedPath, string actualPath, string diffPath) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.AddFileScreenDiff(expectedPath, actualPath, diffPath);

    public void AddGlobalAttachment(string name, Stream content, string? mediaType, string fileExtension) =>
        AllureBackend.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachment(name, content, mediaType, fileExtension);

    public void AddGlobalError(GlobalError error) =>
        AllureBackend.ResolveGlobalScope()?.Operations.Sync.AddGlobalError(error);

    public void AddGlobalFileAttachment(string name, string path, string? mediaType, string fileExtension) =>
        AllureBackend.ResolveGlobalScope()?.Operations.Sync.AddGlobalFileAttachment(name, path, mediaType, fileExtension);

    public void AddLabel(Label label) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.AddLabel(label);

    public void AddLabels(IEnumerable<Label> labels) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.AddLabels(labels);

    public void AddLink(Link link) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.AddLink(link);

    public void AddLinks(IEnumerable<Link> links) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.AddLinks(links);

    public void AddScreenDiff(Stream expected, Stream actual, Stream diff) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.AddScreenDiff(expected, actual, diff);

    public void AddTestParameter(Parameter parameter) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.AddTestParameter(parameter);

    public void SetDescription(string description) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.SetDescription(description);

    public void SetDescriptionHtml(string descriptionHtml) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.SetDescriptionHtml(descriptionHtml);

    public void SetFixtureName(string newName) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.SetFixtureName(newName);

    public void SetLabel(string name, string value) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.SetLabel(name, value);

    public void SetName(string newName) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.SetName(newName);

    public void SetTestName(string newName) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.SetTestName(newName);

    public void SetUp(string name, IEnumerable<Parameter> parameters, Action body)
    {
        if (AllureBackend.ResolveCurrentScope() is { } backend)
        {
            backend.Operations.Sync.SetUp(name, parameters, body);
            return;
        }

        body();
    }

    public void SetUp(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessFixtureContext> body)
    {
        if (AllureBackend.ResolveCurrentScope() is { } backend)
        {
            backend.Operations.Sync.SetUp(name, parameters, body);
            return;
        }

        body(NullOperationContext.Instance);
    }

    public TResult SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Sync.SetUp(name, parameters, body)
            : body();

    public TResult SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContext, TResult> body) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Sync.SetUp(name, parameters, body)
            : body(NullOperationContext.Instance);

    public void Step(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.Step(name, parameters, status, statusDetails);

    public void Step(string name, IEnumerable<Parameter> parameters, Action body)
    {
        if (AllureBackend.ResolveCurrentScope() is { } backend)
        {
            backend.Operations.Sync.Step(name, parameters, body);
            return;
        }

        body();
    }

    public void Step(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessStepContext> body)
    {
        if (AllureBackend.ResolveCurrentScope() is { } backend)
        {
            backend.Operations.Sync.Step(name, parameters, body);
            return;
        }

        body(NullOperationContext.Instance);
    }

    public TResult Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Sync.Step(name, parameters, body)
            : body();

    public TResult Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessStepContext, TResult> body) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Sync.Step(name, parameters, body)
            : body(NullOperationContext.Instance);

    public void TearDown(string name, IEnumerable<Parameter> parameters, Action body)
    {
        if (AllureBackend.ResolveCurrentScope() is { } backend)
        {
            backend.Operations.Sync.TearDown(name, parameters, body);
            return;
        }

        body();
    }

    public void TearDown(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessFixtureContext> body)
    {
        if (AllureBackend.ResolveCurrentScope() is { } backend)
        {
            backend.Operations.Sync.TearDown(name, parameters, body);
            return;
        }

        body(NullOperationContext.Instance);
    }

    public TResult TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Sync.TearDown(name, parameters, body)
            : body();

    public TResult TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContext, TResult> body) =>
        AllureBackend.ResolveCurrentScope() is { } backend
            ? backend.Operations.Sync.TearDown(name, parameters, body)
            : body(NullOperationContext.Instance);

    public bool TryReadFixtureResult<TResult>(Func<FixtureResult, TResult> read, [MaybeNullWhen(false)] out TResult result)
    {
        if (AllureBackend.ResolveCurrentScope() is { } backend)
        {
            return backend.Operations.Sync.TryReadFixtureResult(read, out result);
        }

        result = default;
        return false;
    }

    public bool TryReadStepResult<TResult>(Func<StepResult, TResult> read, [MaybeNullWhen(false)] out TResult result)
    {
        if (AllureBackend.ResolveCurrentScope() is { } backend)
        {
            return backend.Operations.Sync.TryReadStepResult(read, out result);
        }

        result = default;
        return false;
    }

    public bool TryReadTestResult<TResult>(Func<TestResult, TResult> read, [MaybeNullWhen(false)] out TResult result)
    {
        if (AllureBackend.ResolveCurrentScope() is { } backend)
        {
            return backend.Operations.Sync.TryReadTestResult(read, out result);
        }

        result = default;
        return false;
    }

    public void UpdateFixtureResult(Action<FixtureResult> update) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.UpdateFixtureResult(update);

    public void UpdateStepResult(Action<StepResult> update) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.UpdateStepResult(update);

    public void UpdateTestResult(Action<TestResult> update) =>
        AllureBackend.ResolveCurrentScope()?.Operations.Sync.UpdateTestResult(update);

    public static RoutingAllureOperations Instance { get; } = new();
}