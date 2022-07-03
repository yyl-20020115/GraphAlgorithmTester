namespace GraphAlgorithmTester;

public class SNode
{
    public string Name = "";
    public int? Offset = null;
    
    public SNode(string Name = "")
    {
        this.Name = Name;
    }
    public SNode Copy() => new() { Name = this.Name, Offset = this.Offset };
    public SNode Copy(int offset) => new() { Name = this.Name, Offset = offset };
    public override string ToString() => $"{this.Name}{(this.Offset.HasValue ? "("+this.Offset.Value+")" :"") }";
}
