using System;
using System.Collections.Generic;
using System.IO;
using Allure.Model;

namespace Allure.Abstractions;

/// <summary>
/// Defines synchronous operations exposed by an Allure API endpoint.
/// </summary>
/// <typeparam name="TStepContext">
/// The step context type.
/// </typeparam>
/// <typeparam name="TFixtureContext">
/// The fixture context type.
/// </typeparam>
public interface IAllureSyncOperations<out TStepContext, out TFixtureContext>
    where TStepContext : IAllureSyncStepContext
    where TFixtureContext : IAllureSyncFixtureContext
{
    /// <summary>
    /// Sets the name of the current test.
    /// </summary>
    void SetTestName(string newName);

    /// <summary>
    /// Sets the name of the current fixture.
    /// </summary>
    void SetFixtureName(string newName);

    /// <summary>
    /// Sets the name of the current step, fixture, or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the name change.
    /// </remarks>
    void SetName(string newName);

    /// <summary>
    /// Sets the Markdown description of the current test.
    /// </summary>
    void SetDescription(string description);

    /// <summary>
    /// Sets the HTML description of the current test.
    /// </summary>
    void SetDescriptionHtml(string descriptionHtml);

    /// <summary>
    /// Adds labels to the current test.
    /// </summary>
    void AddLabels(IEnumerable<Label> labels);

    /// <summary>
    /// Adds a label to the current test.
    /// </summary>
    void AddLabel(Label label);

    /// <summary>
    /// Sets a label on the current test, replacing labels with the same name.
    /// </summary>
    void SetLabel(string name, string value);

    /// <summary>
    /// Adds links to the current test.
    /// </summary>
    void AddLinks(IEnumerable<Link> links);

    /// <summary>
    /// Adds a link to the current test.
    /// </summary>
    void AddLink(Link link);

    /// <summary>
    /// Adds an attachment to the current step, fixture, or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the attachment.
    /// </remarks>
    void AddAttachment(string name, Stream content, string? mediaType, string fileExtension);

    /// <summary>
    /// Adds an attachment that is not associated with a test or fixture.
    /// </summary>
    void AddGlobalAttachment(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension
    );

    /// <summary>
    /// Adds a screen diff attachment to the current step, fixture, or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the attachment.
    /// </remarks>
    void AddScreenDiff(Stream expected, Stream actual, Stream diff);

    /// <summary>
    /// Adds a file attachment to the current step, fixture, or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the attachment.
    /// </remarks>
    void AddAttachmentFromFile(string name, string path, string? mediaType, string fileExtension);

    /// <summary>
    /// Adds a file attachment that is not associated with a test or fixture.
    /// </summary>
    void AddGlobalAttachmentFromFile(
        string name,
        string path,
        string? mediaType,
        string fileExtension
    );

    /// <summary>
    /// Adds a screen diff attachment from files to the current step, fixture, or test.
    /// </summary>
    /// <remarks>
    /// An implementation must check the step, fixture, and test
    /// contexts in that order. The first one that exists defines
    /// the target of the attachment.
    /// </remarks>
    void AddScreenDiffFromFiles(string expectedPath, string actualPath, string diffPath);

    /// <summary>
    /// Adds an error that is not associated with a test or fixture.
    /// </summary>
    void AddGlobalError(GlobalError error);

    /// <summary>
    /// Adds a parameter to the current test.
    /// </summary>
    void AddTestParameter(Parameter parameter);

    /// <summary>
    /// Adds a completed step.
    /// </summary>
    void Step(
        string name,
        IEnumerable<Parameter> parameters,
        Status status,
        StatusDetails? statusDetails
    );

    /// <summary>
    /// Runs an action as a step.
    /// </summary>
    void Step(string name, IEnumerable<Parameter> parameters, Action body);

    /// <summary>
    /// Runs a context-aware action as a step.
    /// </summary>
    void Step(string name, IEnumerable<Parameter> parameters, Action<TStepContext> body);

    /// <summary>
    /// Runs a function as a step and returns its result.
    /// </summary>
    TResult Step<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body);

    /// <summary>
    /// Runs a context-aware function as a step and returns its result.
    /// </summary>
    TResult Step<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TStepContext, TResult> body
    );

    /// <summary>
    /// Runs a setup fixture.
    /// </summary>
    void SetUp(string name, IEnumerable<Parameter> parameters, Action body);

    /// <summary>
    /// Runs a context-aware setup fixture.
    /// </summary>
    void SetUp(string name, IEnumerable<Parameter> parameters, Action<TFixtureContext> body);

    /// <summary>
    /// Runs a setup fixture and returns its result.
    /// </summary>
    TResult SetUp<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body);

    /// <summary>
    /// Runs a context-aware setup fixture and returns its result.
    /// </summary>
    TResult SetUp<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, TResult> body
    );

    /// <summary>
    /// Runs a teardown fixture.
    /// </summary>
    void TearDown(string name, IEnumerable<Parameter> parameters, Action body);

    /// <summary>
    /// Runs a context-aware teardown fixture.
    /// </summary>
    void TearDown(string name, IEnumerable<Parameter> parameters, Action<TFixtureContext> body);

    /// <summary>
    /// Runs a teardown fixture and returns its result.
    /// </summary>
    TResult TearDown<TResult>(string name, IEnumerable<Parameter> parameters, Func<TResult> body);

    /// <summary>
    /// Runs a context-aware teardown fixture and returns its result.
    /// </summary>
    TResult TearDown<TResult>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<TFixtureContext, TResult> body
    );
}
