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
    abstract class LIB_OperatorFullBase
    {
        public abstract string GetOperateString();
        public void Unuse()
        {
            if (deleteBlock) return;
            HeuristicStateFullInternal.unusedOperatorPool.PushBack(this);
            if (parent != null)
            {
                if (--parent.refCount == 0) parent.Unuse();
            }
        }
        public void SetParent(LIB_OperatorFullBase parent)
        {
            this.parent = parent;
            parent.refCount++;
        }
        public LIB_OperatorFullBase parent;
        public int refCount;
        public bool deleteBlock;
    }
    class LIB_OperatorInitialFull : LIB_OperatorFullBase
    {
        public string operateString;
        public override string GetOperateString()
        {
            return operateString;
        }
    }

    abstract class HeuristicStateFullInternal : IComparable<HeuristicStateFullInternal>
    {
        public int CompareTo(HeuristicStateFullInternal other)
        {
            return 0;
        }

        static public LIB_Deque<LIB_OperatorFullBase> unusedOperatorPool = new LIB_Deque<LIB_OperatorFullBase>();
        public LIB_OperatorFullBase thisOperator;

        public abstract void Initialize(Action<HeuristicStateFullInternal, string> stateRegister);
        public abstract HeuristicStateFullInternal Clone();
        public abstract LIB_OperatorFullBase[] ListupActions(int turn);
        public abstract void DoAction(LIB_OperatorFullBase ope, int thisTurn, out int nextTurn, out long score, out long hash);
        public abstract void Unuse();
    }

    abstract class LIB_HeuristicStateFullBase<TOperator> : HeuristicStateFullInternal where TOperator : LIB_OperatorFullBase, new()
    {
        static public TOperator CreateOperator()
        {
            while (unusedOperatorPool.Count > 0)
            {
                var ret = unusedOperatorPool.PopBack();
                if (ret is TOperator)
                {
                    ret.refCount = 0;
                    ret.parent = null;
                    return (TOperator)ret;
                }
            }
            return new TOperator();
        }
        static LIB_Deque<LIB_HeuristicStateFullBase<TOperator>> unusedStatePool = new LIB_Deque<LIB_HeuristicStateFullBase<TOperator>>();
        public override HeuristicStateFullInternal Clone()
        {
            if (unusedStatePool.Count > 0)
            {
                var ret = unusedStatePool.PopBack();
                ret.thisOperator = null;
                return Clone(ret);
            }
            return Clone(null);
        }
        protected abstract HeuristicStateFullInternal Clone(HeuristicStateFullInternal preObj);
        protected abstract void DoAction(TOperator ope, int thisTurn, out int nextTurn, out long score, out long hash);
        public override void DoAction(LIB_OperatorFullBase ope, int thisTurn, out int nextTurn, out long score, out long hash)
        {
            thisOperator = ope;
            DoAction((TOperator)ope, thisTurn, out nextTurn, out score, out hash);
        }
        public override void Unuse()
        {
            unusedStatePool.PushBack(this);
        }
    }
    ////end
}