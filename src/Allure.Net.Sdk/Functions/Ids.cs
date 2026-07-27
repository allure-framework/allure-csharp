using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Model;

namespace Allure.Sdk.Functions;

public static class Ids
{
    /// <summary>
    /// Generates a UUID for a test or container.
    /// </summary>
    public static string NewUuid() => Guid.NewGuid().ToString();

    /// <summary>
    /// Creates a testCaseId value. testCaseId has a fixed length and depends
    /// only on a given fullName. The fullName shouldn't depend on test parameters.
    /// </summary>
    public static string ForTestCase(string fullName) =>
        Md5.FromString(fullName);

    /// <summary>
    /// Creates a string that uniquely identifies a given test represented by a
    /// test case full name and a sequence of Allure parameters.
    /// The result can be used a <c>historyId</c> value.
    /// </summary>
    /// <param name="fullName">The fullName of a test.</param>
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