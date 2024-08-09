using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Sdk;

public class AiOptions
{
    public string Address { get; private set; }

    public int TimeoutSeconds { get; private set; }

    public AiOptions(string address, int timeoutSeconds = 180)
    {
        Address = address;
        TimeoutSeconds = timeoutSeconds;
    }
}
