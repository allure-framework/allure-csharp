using System;
using Allure.Model;

namespace Allure.Sdk.Functions;

/// <summary>
/// Provides factories for Allure status details.
/// </summary>
public static class StatusDetailsExtensions
{
    extension (StatusDetails)
    {
        /// <summary>
        /// Converts an exception to the status details.
        /// </summary>
        /// <param name="e">The exception to convert.</param>
        public static StatusDetails? FromException(Exception? e) =>
            e is null
                ? null
                : new()
                {
                    Message = string.IsNullOrEmpty(e.Message)
                        ? e.GetType().Name
                        : e.Message,
                    Trace = e.ToString()
                };
    }
}
