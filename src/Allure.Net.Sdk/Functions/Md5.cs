using System.Text;
using System.Text.Json;

namespace Allure.Sdk.Functions;

public static class Md5
{
    public static string FromString(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var outputBytes = md5.ComputeHash(inputBytes);
        return ToHexString(outputBytes);
    }

    internal static string FromObject(object value) => FromString(
        JsonSerializer.Serialize(value)
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