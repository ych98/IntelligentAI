
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace IntelligentAI.Utilities;

public static class JsonUtilities
{
    /// <summary>
    /// Json格式列表 => Html格式Table
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static string ConvertListJsonToHtmlTable(string jsonString)
    {
        try
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            // 尝试将JSON字符串反序列化为列表
            var list = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString, options);

            if (list != null)
            {
                var sb = new StringBuilder();
                sb.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" class=\"el-table__header\" style=\"width: 100%;\">");

                // 添加表头
                sb.Append("<tr>");
                foreach (var key in list[0].Keys)
                {
                    sb.AppendFormat("<th class=\"el-table__cell gutter\">{0}</th>", key);
                }
                sb.Append("</tr>");

                // 添加数据行
                foreach (var item in list)
                {
                    sb.Append("<tr>");
                    foreach (var value in item.Values)
                    {
                        sb.AppendFormat("<td  class=\"el-table__row\" >{0}</td>", value);
                    }
                    sb.Append("</tr>");
                }

                sb.Append("</table>");
                return sb.ToString();
            }
        }
        catch (JsonException)
        {
            // 如果JSON解析失败，尝试反序列化为字典
            return ConvertSingleJsonToHtmlTable(jsonString);
        }
        // 如果列表和字典解析都失败，返回原始字符串
        return jsonString;
    }

    /// <summary>
    /// Json格式文本 => Html格式Table
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static string ConvertSingleJsonToHtmlTable(string jsonString)
    {
        try
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, options);
            if (dictionary != null)
            {
                var sb = new StringBuilder();
                sb.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" class=\"el-table__header\" style=\"width: 100%;\">");

                sb.Append("<tr>");
                foreach (var key in dictionary.Keys)
                {
                    sb.AppendFormat("<th class=\"el-table__cell gutter\">{0}</th>", key);
                }
                sb.Append("</tr>");

                sb.Append("<tr>");
                foreach (var value in dictionary.Values)
                {
                    sb.AppendFormat("<td  class=\"el-table__row\" >{0}</td>", value);
                }
                sb.Append("</tr>");

                sb.Append("</table>");
                return sb.ToString();
            }
        }
        catch (JsonException)
        {
            // 如果JSON解析失败，返回原始字符串
            return jsonString;
        }
        // 如果字典解析失败，返回原始字符串
        return jsonString;
    }

}
