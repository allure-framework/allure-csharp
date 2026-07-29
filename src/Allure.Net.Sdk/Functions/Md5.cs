using System.Text;
using System.Text.Json;

namespace Allure.Sdk.Functions;

/// <summary>
/// Computes lowercase hexadecimal MD5 hashes for stable Allure identifiers.
/// </summary>
public static class Md5
{
    /// <summary>
    /// Computes the MD5 hash of the UTF-8 representation of a string.
    /// </summary>
    /// <param name="input">The string to hash.</param>
    /// <returns>The lowercase hexadecimal hash.</returns>
    public static string FromString(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var outputBytes = md5.ComputeHash(inputBytes);
        return ToHexString(outputBytes);
    }

    /// <summary>
    /// Serializes a value to JSON and computes the MD5 hash of the result.
    /// </summary>
    /// <param name="input">The value to serialize and hash.</param>
    /// <returns>The lowercase hexadecimal hash.</returns>
    public static string FromJson(object input) => FromString(
        JsonSerializer.Serialize(input)
    );

    static string ToHexString(byte[] inputBytes)
    {
        var sb = new StringBuilder();
        foreach (byte b in inputBytes)
        {
            sb.Append(
                b.ToString("x2")
            );
        }
        return sb.ToString();
    }
}
