using System.IO;
using System.Text;

namespace Allure;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure
/// report.
/// </summary>
public static partial class AllureApi
{
    static Stream ToStream(string text) =>
        new MemoryStream(
            Encoding.UTF8.GetBytes(text)
        );
}
