using System;
using System.Collections.Generic;
using System.IO;
using Allure.Model;

namespace Allure.Abstractions;

public interface IAllureTestApi<out TStepContext, out TFixtureContext>
    where TStepContext : IAllureStepContext
    where TFixtureContext : IAllureFixtureContext
{
    void SetTestName(string newName);

    void SetFixtureName(string newName);

    void SetName(string newName);

    void SetDescription(string description);

    void SetDescriptionHtml(string descriptionHtml);

    void AddLabels(IEnumerable<Label> labels);

    void AddLabel(Label label);

    void SetLabel(string name, string value);

    void AddLinks(IEnumerable<Link> links);

    void AddLink(Link link);

    void AddAttachment(string name, Stream content, string? mediaType, string fileExtension);

    void AddGlobalAttachment(string name, Stream content, string? mediaType, string fileExtension);

    void AddScreenDiff(Stream expected, Stream actual, Stream diff);

    void AddFileAttachment(string name, string path, string? mediaType, string fileExtension);

    void AddGlobalFileAttachment(string name, string path, string? mediaType, string fileExtension);

    void AddFileScreenDiff(string expectedPath, string actualPath, string diffPath);

    void AddGlobalError(GlobalError error);

    void AddTestParameter(Parameter parameter);

    void Step(string name, IEnumerable<Parameter> parameters, Status status, StatusDetails? statusDetails);

    void Step(string name, IEnumerable<Parameter> parameters, Action body);

    void Step(string name, IEnumerable<Parameter> parameters, Action<TStepContext> body);

    TResult Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body);

    TResult Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<TStepContext, TResult> body);

    void SetUp(string name, IEnumerable<Parameter> parameters, Action body);

    void SetUp(string name, IEnumerable<Parameter> parameters, Action<TFixtureContext> body);

    TResult SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body);

    TResult SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, TResult> body);

    void TearDown(string name, IEnumerable<Parameter> parameters, Action body);

    void TearDown(string name, IEnumerable<Parameter> parameters, Action<TFixtureContext> body);

    TResult TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body);

    TResult TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<TFixtureContext, TResult> body);
}
