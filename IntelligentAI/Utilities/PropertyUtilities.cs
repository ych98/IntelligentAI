using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Utilities;

public class PropertyUtilities
{
    public static T CheckPropertiesForJson<T>(T exampleInstance)
    {
        if (exampleInstance is null) return default;

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            if (prop.PropertyType != typeof(string)) continue;

            string value = prop.GetValue(exampleInstance) as string;

            if (!string.IsNullOrEmpty(value))
            {
                string htmlTable = JsonUtilities.ConvertListJsonToHtmlTable(value);
                if (!string.IsNullOrEmpty(htmlTable))
                {
                    // 将转换后的HTML表格赋值回原来的属性
                    prop.SetValue(exampleInstance, htmlTable);
                }
            }
        }

        return exampleInstance;
    }
}
