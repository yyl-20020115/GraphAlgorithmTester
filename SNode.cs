namespace GraphAlgorithmTester;

public class SNode
{
    public string Name = "";
    public int NodeIndex = 0;
    public int? Offset = null;
    public int InCapacity = 0;
    public int OutCapacity = 0;
    public int LevelIndex = 0;
    public int DeltaCapacity => this.OutCapacity - this.InCapacity;
    public SNode(string Name = "")
    {
        this.Name = Name;
    }
    public SNode Copy() => new() { Name = this.Name, Offset = this.Offset };
    public SNode Copy(int offset) => new() { Name = this.Name, Offset = offset };
    public override string ToString() => $"{this.Name}{(this.Offset.HasValue ? "("+this.Offset.Value+")" :"") }";
}
