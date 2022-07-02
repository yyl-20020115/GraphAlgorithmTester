using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmTester;

public class SNode
{
    public string Name { get; set; } = string.Empty;
    public double Offset { get; set; }

    public SNode(string Name = "")
    {
        this.Name = Name;
    }
    public SNode Copy() => new() { Name = this.Name, Offset = this.Offset };
    public SNode Copy(double offset) => new() { Name = this.Name, Offset = offset };
    public override string ToString() => $"{this.Name}({this.Offset})";
}
