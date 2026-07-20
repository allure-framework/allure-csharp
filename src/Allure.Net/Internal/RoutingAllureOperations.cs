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
        AllureBackend.CurrentBackend?.TestApi.Sync.AddAttachment(name, content, mediaType, fileExtension);

    public void AddFileAttachment(string name, string path, string? mediaType, string fileExtension) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddFileAttachment(name, path, mediaType, fileExtension);

    public void AddFileScreenDiff(string expectedPath, string actualPath, string diffPath) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddFileScreenDiff(expectedPath, actualPath, diffPath);

    public void AddGlobalAttachment(string name, Stream content, string? mediaType, string fileExtension) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddGlobalAttachment(name, content, mediaType, fileExtension);

    public void AddGlobalError(GlobalError error) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddGlobalError(error);

    public void AddGlobalFileAttachment(string name, string path, string? mediaType, string fileExtension) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddGlobalFileAttachment(name, path, mediaType, fileExtension);

    public void AddLabel(Label label) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddLabel(label);

    public void AddLabels(IEnumerable<Label> labels) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddLabels(labels);

    public void AddLink(Link link) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddLink(link);

    public void AddLinks(IEnumerable<Link> links) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddLinks(links);

    public void AddScreenDiff(Stream expected, Stream actual, Stream diff) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddScreenDiff(expected, actual, diff);

    public void AddTestParameter(Parameter parameter) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.AddTestParameter(parameter);

    public void SetDescription(string description) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.SetDescription(description);

    public void SetDescriptionHtml(string descriptionHtml) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.SetDescriptionHtml(descriptionHtml);

    public void SetFixtureName(string newName) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.SetFixtureName(newName);

    public void SetLabel(string name, string value) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.SetLabel(name, value);

    public void SetName(string newName) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.SetName(newName);

    public void SetTestName(string newName) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.SetTestName(newName);

    public void SetUp(string name, IEnumerable<Parameter> parameters, Action body)
    {
        if (AllureBackend.CurrentBackend is { } backend)
        {
            backend.TestApi.Sync.SetUp(name, parameters, body);
            return;
        }

        body();
    }

    public void SetUp(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessFixtureContext> body)
    {
        if (AllureBackend.CurrentBackend is { } backend)
        {
            backend.TestApi.Sync.SetUp(name, parameters, body);
            return;
        }

        body(NullOperationContext.Instance);
    }

    public TResult SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Sync.SetUp(name, parameters, body)
            : body();

    public TResult SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContext, TResult> body) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Sync.SetUp(name, parameters, body)
            : body(NullOperationContext.Instance);

    public void Step(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.Step(name, parameters, status, statusDetails);

    public void Step(string name, IEnumerable<Parameter> parameters, Action body)
    {
        if (AllureBackend.CurrentBackend is { } backend)
        {
            backend.TestApi.Sync.Step(name, parameters, body);
            return;
        }

        body();
    }

    public void Step(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessStepContext> body)
    {
        if (AllureBackend.CurrentBackend is { } backend)
        {
            backend.TestApi.Sync.Step(name, parameters, body);
            return;
        }

        body(NullOperationContext.Instance);
    }

    public TResult Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Sync.Step(name, parameters, body)
            : body();

    public TResult Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessStepContext, TResult> body) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Sync.Step(name, parameters, body)
            : body(NullOperationContext.Instance);

    public void TearDown(string name, IEnumerable<Parameter> parameters, Action body)
    {
        if (AllureBackend.CurrentBackend is { } backend)
        {
            backend.TestApi.Sync.TearDown(name, parameters, body);
            return;
        }

        body();
    }

    public void TearDown(string name, IEnumerable<Parameter> parameters, Action<IAllureInProcessFixtureContext> body)
    {
        if (AllureBackend.CurrentBackend is { } backend)
        {
            backend.TestApi.Sync.TearDown(name, parameters, body);
            return;
        }

        body(NullOperationContext.Instance);
    }

    public TResult TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Sync.TearDown(name, parameters, body)
            : body();

    public TResult TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<IAllureInProcessFixtureContext, TResult> body) =>
        AllureBackend.CurrentBackend is { } backend
            ? backend.TestApi.Sync.TearDown(name, parameters, body)
            : body(NullOperationContext.Instance);

    public bool TryReadFixtureResult<TResult>(Func<FixtureResult, TResult> read, [MaybeNullWhen(false)] out TResult result)
    {
        if (AllureBackend.CurrentBackend is { } backend)
        {
            return backend.TestApi.Sync.TryReadFixtureResult(read, out result);
        }

        result = default;
        return false;
    }

    public bool TryReadStepResult<TResult>(Func<StepResult, TResult> read, [MaybeNullWhen(false)] out TResult result)
    {
        if (AllureBackend.CurrentBackend is { } backend)
        {
            return backend.TestApi.Sync.TryReadStepResult(read, out result);
        }

        result = default;
        return false;
    }

    public bool TryReadTestResult<TResult>(Func<TestResult, TResult> read, [MaybeNullWhen(false)] out TResult result)
    {
        if (AllureBackend.CurrentBackend is { } backend)
        {
            return backend.TestApi.Sync.TryReadTestResult(read, out result);
        }

        result = default;
        return false;
    }

    public void UpdateFixtureResult(Action<FixtureResult> update) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.UpdateFixtureResult(update);

    public void UpdateStepResult(Action<StepResult> update) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.UpdateStepResult(update);

    public void UpdateTestResult(Action<TestResult> update) =>
        AllureBackend.CurrentBackend?.TestApi.Sync.UpdateTestResult(update);

    public static RoutingAllureOperations Instance { get; } = new();
}