
using System.Text.RegularExpressions;

namespace IntelligentAI.Utilities;

public static class HtmlUtilities
{
    /// <summary>
    /// 获取HTML标签内容
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