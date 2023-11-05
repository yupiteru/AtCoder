
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Library
{
    ////start
    class LIB_MoyasuUmeru
    {
        const long INF = 1000000000000000;
        public class AddVertexHelper
        {
            long vtx;
            LIB_MoyasuUmeru master;
            public AddVertexHelper(long vtx, LIB_MoyasuUmeru master)
            {
                this.vtx = vtx;
                this.master = master;
            }
            public AddVertexHelper LeftCost(long cost)
            {
                master.path[vtx][master.goal] += cost;
                return this;
            }
            public AddVertexHelper LeftGain(long gain)
            {
                master.shift += gain;
                master.path[master.start][vtx] += gain;
                return this;
            }
            public AddVertexHelper RightCost(long cost)
            {
                master.path[master.start][vtx] += cost;
                return this;
            }
            public AddVertexHelper RightGain(long gain)
            {
                master.shift += gain;
                master.path[vtx][master.goal] += gain;
                return this;
            }
        }
        public class AddConditionHelper
        {
            long tmpvtx;
            List<long> leftList;
            List<long> rightList;
            LIB_MoyasuUmeru master;
            public AddConditionHelper(long vtx, LIB_MoyasuUmeru master)
            {
                tmpvtx = vtx;
                leftList = new List<long>();
                rightList = new List<long>();
                this.master = master;
            }
            public AddConditionHelper And(long vtx)
            {
                tmpvtx = vtx;
                return this;
            }
            public AddConditionHelper IsLeft()
            {
                leftList.Add(tmpvtx);
                return this;
            }
            public AddConditionHelper IsRight()
            {
                rightList.Add(tmpvtx);
                return this;
            }
            public void ThenBanned() => ThenCost(INF);
            public void ThenCost(long cost)
            {
                if (leftList.Count == 1 && rightList.Count == 1)
                {
                    master.path[leftList[0]][rightList[0]] = cost;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
        long shift;
        LIB_Dict<long, LIB_Dict<long, long>> path;
        long additionalVertexCount;
        long start;
        long goal;
        public LIB_MoyasuUmeru()
        {
            path = new LIB_Dict<long, LIB_Dict<long, long>>(_ => new LIB_Dict<long, long>());
            shift = 0;
            start = -1;
            goal = -2;
            additionalVertexCount = 2;
        }
        public AddVertexHelper AddVertex(long vtx)
        {
            path[start][vtx] = 0;
            path[vtx][goal] = 0;
            return new AddVertexHelper(vtx, this);
        }
        public AddConditionHelper AddCondition(long vtx)
        {
            return new AddConditionHelper(vtx, this);
        }
        public long Calc()
        {
            var totalVtxCount = path.Keys.Concat(path.SelectMany(e => e.Value.Keys)).Distinct().Count();
            var flow = new LIB_MaxFlow(totalVtxCount);
            var additionalBase = totalVtxCount - additionalVertexCount - 1;
            var flowStart = additionalBase + -start;
            var flowGoal = additionalBase + -goal;
            foreach (var item in path)
            {
                var from = item.Key;
                if (from < 0) from = additionalBase + -from;
                foreach (var item2 in item.Value)
                {
                    var to = item2.Key;
                    if (to < 0) to = additionalBase + -to;
                    flow.AddEdge(from, to, item2.Value);
                }
            }
            return shift - flow.Flow(flowStart, flowGoal);
        }
    }
    ////end
}