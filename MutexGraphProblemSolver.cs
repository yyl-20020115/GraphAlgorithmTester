using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmTester;

public class MutexGraphProblemSolver : ProblemSolver
{
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        //# 元素=[1,2,3,4,5,6,7,8,9] 互斥=[(1,4),(2,5),(1,5),(5,6),(7,8),(3,9),(2,8),(4,5)]
        //# 把元素组成 N 个组, 保证互斥元素不在同一个组里, 并且 N 最小

    }
}
