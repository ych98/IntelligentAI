using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text.Json;
namespace IntelligentAI.Utilities;

public static class VerificationUtilities
{
    /// <summary>
    /// 检测URL格式是否符合
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool VerifyUrlFormat(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        // 检验Url格式是否符合
        if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            return true;
        return false;
    }

    /// <summary>
    /// 检测URL格式是否符合
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool VerifyUrlFormatByRegex(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        // 检验Url格式是否符合
        Regex regex = new Regex(@"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?", RegexOptions.IgnoreCase);

        return regex.IsMatch(url);
    }

    /// <summary>
    /// 检测是否为html标签
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static bool VerifyHtmlFormat(string html)
    {
        Regex regex = new Regex(@"<[\s\S]*?>");

        return regex.IsMatch(html);
    }

    /// <summary>
    /// 校验html是否为引用格式
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static bool IsReferenceHtml(this string html)
    {
        Regex regex = new Regex("<a[^>]*>([\\s\\S]*?)</a>");

        return regex.IsMatch(html);
    }

    /// <summary>
    /// 校验html是否为加粗格式
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static bool IsStrongHtml(string html)
    {
        Regex regex = new Regex("<strong[^>]*>([\\s\\S]*?)</strong>");

        return regex.IsMatch(html);
    }

    /// <summary>
    /// 校验html是否为图片格式
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static bool IsImageHtml(string html)
    {
        string defaultImageRegex = @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*/?[\s\t\r\n]*>";

        Regex regex = new Regex(defaultImageRegex);

        return regex.IsMatch(html);
    }

    /// <summary>
    /// 判断文件名称是否符合命名规则
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool VerifyFileName(string fileName)
    {
        // 文件名称不能包含以下字符：\ / : * ? " < > |
        string invalidChars = @"\/:*?""<>|";
        // 使用正则表达式，匹配文件名称中是否包含无效字符
        Regex regex = new Regex("[" + Regex.Escape(invalidChars) + "]");
        // 如果匹配成功，则返回false，否则返回true
        return !regex.IsMatch(fileName);
    }

    /// <summary>
    /// 校验手机号是否符合规则
    /// </summary>
    /// <param name="phone"></param>
    /// <returns></returns>
    public static bool VerifyPhoneNumberFormat(string phone)
    {
        Regex regex = new Regex(@"^1[3-9]\d{9}$");

        return regex.IsMatch(phone);
    }

    /// <summary>
    /// 校验邮箱是否符合规则
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool VerifyEmailFormat(string email)
    {
        Regex regex = new Regex(@"^\s*([A-Za-z0-9_-]+(\.\w+)*@(\w+\.)+\w{2,5})\s*$");

        return regex.IsMatch(email);
    }

    /// <summary>
    /// 检测端口是否被占用
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public static bool IsPortAlreadyInUse(int port)
    {
        // 获取本机所有的监听连接
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
        foreach (IPEndPoint endPoint in ipEndPoints)
        {
            if (endPoint.Port == port) return true;
        }
        return false;
    }

    /// <summary>
    /// 校验消息体内容是否被篡改
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="body"></param>
    /// <param name="checksum"></param>
    /// <returns></returns>
    public static bool VerifyChecksum<T>(T body, string checksum)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body, options)));
            return checksum == BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }

    /// <summary>
    /// 判断元素是否存在列表中，一般用于校验前后端值是否匹配
    /// </summary>
    /// <param name="target"></param>
    /// <param name="elements"></param>
    /// <returns></returns>
    public static bool IsElementInList(string target, params string[] elements) => elements.Contains(target);
}