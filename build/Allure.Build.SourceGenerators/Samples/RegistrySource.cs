using System.Collections.Immutable;

namespace Allure.Build.SourceGenerators.Samples;

sealed record RegistrySource(string Namespace, ImmutableArray<RegistryEntry> Entries);
