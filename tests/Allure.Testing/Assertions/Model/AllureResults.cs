using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;

namespace Allure.Testing.Assertions.Model;

public record class AllureResults2(
    ImmutableArray<JsonElement> TestResults,
    ImmutableArray<JsonElement> Containers,
    ImmutableDictionary<string, ReadOnlyMemory<byte>> Attachments
);
