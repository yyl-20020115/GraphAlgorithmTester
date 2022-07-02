# GraphAlgorithmTester
NP=P as proven by traveller problem

HOW THIS ALGORITHM WORKS?

Just simply follow the graph parallelly and split the path when there is a branch point(node).
After all possible paths are collected, measure the total length of the edges.

Finally, sort by distance and then you get the solutions of the traveller problem.

It can be treated as a proof for "NP=P".

PLEASE READ THE CODE FOR DETAILS! (C#)

Best Regards,
Yilin

以并行算法证明 NP=P

推销员旅行问题的并行算法简介：

从初始城市开始，并行跟踪链接它的所有的边，到可能去到的所有城市，到达之后再基于这个城市跟踪所有的边，
到达它之后的所有城市，这时候需要一个关于路径的记录对象（记录所到过的城市以及城市间的距离并更新当前路径的长度），
而遇到城市之后，路径会分裂，一个路径变长的过程中，
也变成多个分裂的路径。删除那些“上一步”留下的路径，只保留最新的路径。直到路径遇到初始城市为止。
过程中可能会收取大量的路径对象，但最终它们都是首尾相接的闭合路径，而且除了首尾之外，节点都不重复
（不重复经过任何中间城市），且路径包含城市的个数等于城市总数加一。
获得这些闭合路径之后，再根据这些路径的物理长度排序，选取最短的就是想要的结果。

这种算法，实际上只需要城市数量那么多的步骤，也就是说，线性时间O(n)即可解决。

详细算法请阅读代码。

逸霖
