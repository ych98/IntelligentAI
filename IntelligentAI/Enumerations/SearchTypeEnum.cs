using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class SearchTypeEnum : Enumeration
{
    public static SearchTypeEnum AiSearch = new SearchTypeEnum(1, nameof(AiSearch), "AI搜索");

    public static SearchTypeEnum MohuSearch = new SearchTypeEnum(2, nameof(MohuSearch), "模糊搜索");

    public static SearchTypeEnum MrcSearch= new SearchTypeEnum(3, nameof(MrcSearch), "MRC模型搜索");

    public static SearchTypeEnum VectorSearch = new SearchTypeEnum(4, nameof(VectorSearch), "向量搜索(仅支持法规库)");

    public SearchTypeEnum(int id, string name, string description) : base(id, name, description) { }
    
    public static SearchTypeEnum GetById(int id) => FromId<SearchTypeEnum>(id);
    public static SearchTypeEnum GetByName(string name) => FromName<SearchTypeEnum>(name);
    public static SearchTypeEnum GetByDescription(string description) => FromDescription<SearchTypeEnum>(description);
}
