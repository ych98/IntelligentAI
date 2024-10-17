using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class ServiceEnum : Enumeration
{
    public static ServiceEnum OpenAI = new ServiceEnum(1, nameof(OpenAI));

    public static ServiceEnum Kimi = new ServiceEnum(2, nameof(Kimi));

    public static ServiceEnum Google = new ServiceEnum(3, nameof(Google));

    public static ServiceEnum Aliyun = new ServiceEnum(4, nameof(Aliyun));

    public static ServiceEnum Azure = new ServiceEnum(5, nameof(Azure));

    public static ServiceEnum Huoshan = new ServiceEnum(6, nameof(Huoshan), "抖音火山引擎服务");

    public static ServiceEnum Baidu = new ServiceEnum(7, nameof(Baidu), "百度千帆大模型服务");

    public ServiceEnum(int id, string name,string description) : base(id, name, description: description) { }

    public ServiceEnum(int id, string name) : base(id, name, $"{name} model service") { }

    public static ServiceEnum GetByName(string name) => FromName<ServiceEnum>(name);
}
