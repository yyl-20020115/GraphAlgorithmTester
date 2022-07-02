using System.Collections.Generic;
using System.IO;

namespace GraphAlgorithmTester;

public static class Utils
{
    public static void BuildGraph(string path, SortedDictionary<string, SNode> nodes, HashSet<SEdge> edges)
    {
        using var reader = new StreamReader(path);
        while (reader.ReadLine() is string line)
        {
            if ((line = line.Trim()).StartsWith('#')) continue;
            var dn = 0;
            var p = line.IndexOf("->"); //directional
            if (p >= 0) //this is edge
            {
                var so = line[..p].Trim();
                var st = line[(p + 2)..].Trim();
                var sp = st.IndexOf(':');
                if (sp >= 0)
                {
                    var dt = st[(sp + 1)..];
                    st = st[..(sp)];
                    if (int.TryParse(dt, out dn))
                    {
                        //ok
                    }
                }
                if (!nodes.TryGetValue(so, out var o))
                    nodes.Add(so, o = new SNode(so));
                if (!nodes.TryGetValue(st, out var t))
                    nodes.Add(st, t = new SNode(st));
                edges.Add(new SEdge(o, t, dn));
            }
            else if ((p = line.IndexOf('-')) >= 0)
            {
                var so = line[..p].Trim();
                var st = line[(p + 1)..].Trim(); //indirectional
                var sp = st.IndexOf(':');
                if (sp >= 0)
                {
                    var dt = st[(sp + 1)..];
                    st = st[..(sp)];
                    if (int.TryParse(dt, out dn))
                    {
                        //ok
                    }
                }

                if (!nodes.TryGetValue(so, out var o))
                    nodes.Add(so, o = new SNode(so));
                if (!nodes.TryGetValue(st, out var t))
                    nodes.Add(st, t = new SNode(st));
                edges.Add(new SEdge(o, t, dn));
                edges.Add(new SEdge(t, o, dn));
            }
            else //this is node 
            {
                if (!nodes.TryGetValue(line, out var n))
                    nodes.Add(line, n = new SNode(line));
            }
        }
    }
}
