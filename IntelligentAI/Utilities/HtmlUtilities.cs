
using System.Text.RegularExpressions;

namespace IntelligentAI.Utilities;

public static class HtmlUtilities
{
    /// <summary>
    /// ��ȡHTML��ǩ����
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static string GetHtmlValue(string html)
    {
        Regex regex = new Regex("<.*?>");

        var content = regex.Replace(html, "");

        return content;
    }
}