using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Components.Abstraction;

public class WorkflowComponentBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<string, object> InputParameters { get; set; }
    public object OutputResult { get; set; }
    public Func<Task<object>> RunCommandAsync { get; set; }
}
