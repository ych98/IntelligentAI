using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class TextFromImageEnum : Enumeration
{
    public static TextFromImageEnum Title = new TextFromImageEnum(1, nameof(Title), "图片标题生成");

    public static TextFromImageEnum Content = new TextFromImageEnum(2, nameof(Content), "图片内容描述");

    public TextFromImageEnum(int id, string name, string description) : base(id, name, description) { }

    public static TextFromImageEnum GetById(int id) => FromId<TextFromImageEnum>(id);

    public static TextFromImageEnum GetByName(string name) => FromName<TextFromImageEnum>(name);
}
