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
    /// ���URL��ʽ�Ƿ����
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool VerifyUrlFormat(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        // ����Url��ʽ�Ƿ����
        if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            return true;
        return false;
    }

    /// <summary>
    /// ���URL��ʽ�Ƿ����
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool VerifyUrlFormatByRegex(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        // ����Url��ʽ�Ƿ����
        Regex regex = new Regex(@"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?", RegexOptions.IgnoreCase);

        return regex.IsMatch(url);
    }

    /// <summary>
    /// ����Ƿ�Ϊhtml��ǩ
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static bool VerifyHtmlFormat(string html)
    {
        Regex regex = new Regex(@"<[\s\S]*?>");

        return regex.IsMatch(html);
    }

    /// <summary>
    /// У��html�Ƿ�Ϊ���ø�ʽ
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static bool IsReferenceHtml(this string html)
    {
        Regex regex = new Regex("<a[^>]*>([\\s\\S]*?)</a>");

        return regex.IsMatch(html);
    }

    /// <summary>
    /// У��html�Ƿ�Ϊ�Ӵָ�ʽ
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static bool IsStrongHtml(string html)
    {
        Regex regex = new Regex("<strong[^>]*>([\\s\\S]*?)</strong>");

        return regex.IsMatch(html);
    }

    /// <summary>
    /// У��html�Ƿ�ΪͼƬ��ʽ
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
    /// �ж��ļ������Ƿ������������
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool VerifyFileName(string fileName)
    {
        // �ļ����Ʋ��ܰ��������ַ���\ / : * ? " < > |
        string invalidChars = @"\/:*?""<>|";
        // ʹ��������ʽ��ƥ���ļ��������Ƿ������Ч�ַ�
        Regex regex = new Regex("[" + Regex.Escape(invalidChars) + "]");
        // ���ƥ��ɹ����򷵻�false�����򷵻�true
        return !regex.IsMatch(fileName);
    }

    /// <summary>
    /// У���ֻ����Ƿ���Ϲ���
    /// </summary>
    /// <param name="phone"></param>
    /// <returns></returns>
    public static bool VerifyPhoneNumberFormat(string phone)
    {
        Regex regex = new Regex(@"^1[3-9]\d{9}$");

        return regex.IsMatch(phone);
    }

    /// <summary>
    /// У�������Ƿ���Ϲ���
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool VerifyEmailFormat(string email)
    {
        Regex regex = new Regex(@"^\s*([A-Za-z0-9_-]+(\.\w+)*@(\w+\.)+\w{2,5})\s*$");

        return regex.IsMatch(email);
    }

    /// <summary>
    /// ���˿��Ƿ�ռ��
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public static bool IsPortAlreadyInUse(int port)
    {
        // ��ȡ�������еļ�������
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
        foreach (IPEndPoint endPoint in ipEndPoints)
        {
            if (endPoint.Port == port) return true;
        }
        return false;
    }

    /// <summary>
    /// У����Ϣ�������Ƿ񱻴۸�
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
    /// �ж�Ԫ���Ƿ�����б��У�һ������У��ǰ���ֵ�Ƿ�ƥ��
    /// </summary>
    /// <param name="target"></param>
    /// <param name="elements"></param>
    /// <returns></returns>
    public static bool IsElementInList(string target, params string[] elements) => elements.Contains(target);
}