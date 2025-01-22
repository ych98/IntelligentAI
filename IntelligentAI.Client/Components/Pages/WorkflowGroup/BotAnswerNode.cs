using Blazor.Diagrams.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Client.Components.Pages.WorkflowGroup;


public class BotAnswerNode : NodeModel
{
    public BotAnswerNode(Blazor.Diagrams.Core.Geometry.Point position = null) : base(position) { }

    public string Answer { get; set; }
}
