using System;
using System.Collections.Immutable;
using System.Text.Json;

namespace Allure.Testing.Assertions.Model;

public record class AllureResults2(
    ImmutableArray<AllureTestResult> TestResults,
    ImmutableArray<AllureContainer> Containers,
    ImmutableDictionary<string, ReadOnlyMemory<byte>> Attachments
);
