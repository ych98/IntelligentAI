using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class ApiEnum : Enumeration
{
    public static ApiEnum AzureService = new ApiEnum(1, nameof(AzureService), "微软大模型服务");
    public static ApiEnum AliyunService = new ApiEnum(2, nameof(AliyunService), "Aliyun服务");
    public static ApiEnum KimiService = new ApiEnum(3, nameof(KimiService), "Kimi服务");
    public static ApiEnum HuoshanService = new ApiEnum(4, nameof(HuoshanService), "火山引擎服务");
    public static ApiEnum BaiduService = new ApiEnum(5, nameof(BaiduService), "百度千帆大模型服务");

    public ApiEnum(int id, string name, string description) : base(id, name, description) { }

    public static ApiEnum GetById(int id) => FromId<ApiEnum>(id);
    
    public static ApiEnum GetByName(string name) => FromName<ApiEnum>(name);
}
