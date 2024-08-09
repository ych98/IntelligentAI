using IntelligentAI.Models;
using IntelligentAI.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Abstraction;

public interface IAiModelFactory
{
    AiModelBase CreateModel(string serviceName, string modelName);
}
