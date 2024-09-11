using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace IntelligentAI.Utilities;

public static class TextUtilities
{
    /// <summary>
    /// 文本保留，默认4K
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string Chunk(string content, int chunkSize = 4096)
    {
        byte[] contentBytes = Encoding.UTF8.GetBytes(content);

        long contentSize = contentBytes.Length;

        if (contentSize <= chunkSize) return content;

        byte[] chunkBytes = new byte[chunkSize];

        // 从markdownBytes中复制数据到chunkBytes
        Array.Copy(contentBytes, 0, chunkBytes, 0, chunkSize);

        // 将字节数组转换回字符串
        string chunk = Encoding.UTF8.GetString(chunkBytes);

        return chunk;
    }

    /// <summary>
    /// 去除文本中的转义字符
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string EscapePattern(string content)
    {
        string escapePattern = @"&[a-zA-Z0-9#]+;";

        // 使用正则表达式和方法进行转换
        return Regex.Replace(content, escapePattern, ReplaceEscape);
    }

    // 转换转义字符的方法
    private static string ReplaceEscape(Match m)
        => m.Value switch
        {
            "&nbsp;" => " ",
            "&#160;" => " ",
            "&amp;" => "&",
            "&#38;" => "&",
            "&lt;" => "<",
            "&#60;" => "<",
            "&gt;" => ">",
            "&#62;" => ">",
            "&quot;" => "\"",
            "&#34;" => "\"",

            "&apos;" => "'",
            "&ldquo;" => "“",
            "&rdquo;" => "”",
            "&lsquo;" => "‘",
            "&rsquo;" => "’",
            "&hellip;" => "…",
            "&middot;" => "·",
            "&emsp;" => "  ",
            "&mdash;" => "-",

            "&iexcl;" => "¡",
            "&#161;" => "¡",
            "&cent;" => "¢",
            "&#162;" => "¢",
            "&pound;" => "£",
            "&#163;" => "£",
            "&copy;" => "©",
            "&#169;" => "©",
            _ => m.Value,
        };

    // 使用 AES 加密算法加密密码
    public static string EncryptPassword(string password, string key = "Random@123", string iv = "random")
    {
        if (string.IsNullOrWhiteSpace(password)) return string.Empty;

        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = CreateKey(key);
        aes.IV = CreateIv(iv);
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var encryptedBytes = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(password), 0, password.Length);
        return Convert.ToBase64String(encryptedBytes);
    }

    // 使用 AES 加密算法解密密码
    public static string DecryptPassword(string encryptedPassword, string key = "Random@123", string iv = "random")
    {
        if (string.IsNullOrWhiteSpace(encryptedPassword)) return string.Empty;

        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = CreateKey(key);
        aes.IV = CreateIv(iv);
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        try
        {
            var encryptedBytes = Convert.FromBase64String(encryptedPassword);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception)
        {
            return encryptedPassword;
        }
    }

    private static byte[] CreateKey(string key)
    {
        // 使用SHA256散列算法来生成一个32字节的密钥
        SHA256 sha256 = SHA256.Create();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] hashedKey = sha256.ComputeHash(keyBytes);
        return hashedKey;
    }

    private static byte[] CreateIv(string iv)
    {
        // 使用MD5散列算法来生成一个16字节的IV
        MD5 md5 = MD5.Create();
        byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
        byte[] hashedIv = md5.ComputeHash(ivBytes);
        return hashedIv;
    }
}
