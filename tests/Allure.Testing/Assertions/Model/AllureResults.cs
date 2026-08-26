using System;
using System.Collections.Immutable;

namespace Allure.Testing.Assertions.Model;

public record class AllureResults(
    ImmutableArray<AllureTestResult> TestResults,
    ImmutableArray<AllureContainer> Containers,
    ImmutableDictionary<string, ReadOnlyMemory<byte>> Attachments,
    ImmutableArray<AllureGlobals> Globals
);
