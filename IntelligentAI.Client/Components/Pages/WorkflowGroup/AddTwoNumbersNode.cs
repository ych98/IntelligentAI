using Blazor.Diagrams.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Client.Components.Pages.WorkflowGroup;

public class AddTwoNumbersNode : NodeModel
{
    public AddTwoNumbersNode(Blazor.Diagrams.Core.Geometry.Point position = null) : base(position) { }

    public double FirstNumber { get; set; }
    public double SecondNumber { get; set; }

    // Here, you can put whatever you want, such as a method that does the addition
}