using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Model;

namespace Allure.Sdk.Functions;

/// <summary>
/// Generates identifiers used by Allure result objects.
/// </summary>
public static class Ids
{
    /// <summary>
    /// Generates a UUID for a test or container.
    /// </summary>
    public static string NewUuid() => Guid.NewGuid().ToString();

    /// <summary>
    /// Creates a fixed-length test case ID derived from a test's full name.
    /// </summary>
    public static string ForTestCase(string fullName) =>
        Md5.FromString(fullName);

    /// <summary>
    /// Creates a string that uniquely identifies a given test represented by a
    /// test case full name and a sequence of Allure parameters.
    /// The result can be used as a history ID.
    /// </summary>
    /// <param name="fullName">The full name of a test.</param>
    /// <param name="parameters">The parameters of a test.</param>
    public static string ForTest(
        string fullName,
        IEnumerable<Parameter> parameters
    ) =>
        Md5.FromJson(new
        {
            fullName,
            parameters = parameters.Where(p => !p.Excluded)
                .OrderBy(p => p.Name)
                .Select(p => p.Value)
        });
}
