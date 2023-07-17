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
    class LIB_LPSimplex
    {
        public class Subject
        {
            public LIB_Fraction[] ary;
            public LIB_Fraction value;
            public bool hasSlack;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Subject(LIB_Fraction[] ary)
            {
                this.ary = ary;
                value = 0;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void EqualTo(LIB_Fraction val)
            {
                // TODO public で呼ばれるとスラック変数が無いので基底決め打ちができない
                value = val;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void LessThan(LIB_Fraction val)
            {
                hasSlack = true;
                EqualTo(val);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GreaterThan(LIB_Fraction val)
            {
                for (var i = 0; i < ary.Length; ++i) ary[i] *= -1;
                LessThan(val * -1);
            }
        }

        List<Subject> subs;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LPSimplex()
        {
            subs = new List<Subject>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Subject AddSubject(params LIB_Fraction[] ary)
        {
            var sub = new Subject(ary.ToArray());
            subs.Add(sub);
            return sub;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void NonNegative(params long[] idx)
        {
            // TODO 自由変数の取り扱い
            if (idx.Length == 0) return;
            var ary = Enumerable.Repeat(0, (int)idx.Max() + 1).Select(e => new LIB_Fraction(0, 1)).ToArray();
            foreach (var item in idx)
            {
                ary[item] = 1;
                AddSubject(ary).GreaterThan(0);
                ary[item] = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Fraction Minimize(params LIB_Fraction[] ary)
        {
            var maxlen = subs.Max(e => e.ary.Length);

            var target = new LIB_Fraction[maxlen + subs.Count];
            for (var i = 0; i < ary.Length; ++i)
            {
                target[i] = ary[i];
            }
            var targetConstant = new LIB_Fraction(0, 1);
            var kitei = new long[subs.Count];
            var subjects = new LIB_Fraction[subs.Count][];
            var constants = new LIB_Fraction[subs.Count];
            for (var i = 0; i < subs.Count; ++i)
            {
                var sub = subs[i];
                kitei[i] = maxlen + i;
                subjects[i] = new LIB_Fraction[maxlen + subs.Count];
                constants[i] = sub.value;
                for (var j = 0; j < sub.ary.Length; ++j)
                {
                    subjects[i][j] = sub.ary[j];
                }
                subjects[i][kitei[i]] = 1;
            }

            while (true)
            {
                var minidx = -1;
                var minvalue = new LIB_Fraction(0, 1);
                for (var i = 0; i < target.Length; ++i)
                {
                    if (minvalue > target[i])
                    {
                        minidx = i;
                        minvalue = target[i];
                    }
                }
                if (minidx == -1) break;

                minvalue = new LIB_Fraction(1, 0);
                var selectedSubIdx = -1;
                for (var i = 0; i < subjects.Length; ++i)
                {
                    var zouka = constants[i] / subjects[i][minidx];
                    if (zouka < 0) continue;
                    if (minvalue > zouka)
                    {
                        minvalue = zouka;
                        selectedSubIdx = i;
                    }
                }
                kitei[selectedSubIdx] = minidx;

                var selectedSub = subjects[selectedSubIdx];
                var mul = 1 / selectedSub[minidx];
                for (var i = 0; i < selectedSub.Length; ++i)
                {
                    selectedSub[i] *= mul;
                }
                constants[selectedSubIdx] *= mul;

                for (var i = 0; i < subjects.Length; ++i)
                {
                    if (i == selectedSubIdx) continue;
                    mul = subjects[i][minidx];
                    for (var j = 0; j < subjects[i].Length; ++j)
                    {
                        subjects[i][j] -= selectedSub[j] * mul;
                    }
                    constants[i] -= constants[selectedSubIdx] * mul;
                }
                mul = target[minidx];
                for (var i = 0; i < target.Length; ++i)
                {
                    target[i] -= selectedSub[i] * mul;
                }
                targetConstant -= constants[selectedSubIdx] * mul;
            }

            return -1 * targetConstant;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Fraction Maximize(params LIB_Fraction[] ary)
        {
            ary = ary.ToArray();
            for (var i = 0; i < ary.Length; ++i) ary[i] *= -1;
            return -1 * Minimize(ary);
        }
    }
    class LIB_LPSimplexDouble
    {
        public class Subject
        {
            public double[] ary;
            public double value;
            public bool hasSlack;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Subject(double[] ary)
            {
                this.ary = ary;
                value = 0;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void EqualTo(double val)
            {
                // TODO public で呼ばれるとスラック変数が無いので基底決め打ちができない
                value = val;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void LessThan(double val)
            {
                hasSlack = true;
                EqualTo(val);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GreaterThan(double val)
            {
                for (var i = 0; i < ary.Length; ++i) ary[i] *= -1;
                LessThan(val * -1);
            }
        }

        List<Subject> subs;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LPSimplexDouble()
        {
            subs = new List<Subject>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Subject AddSubject(params double[] ary)
        {
            var sub = new Subject(ary.ToArray());
            subs.Add(sub);
            return sub;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void NonNegative(params long[] idx)
        {
            // TODO 自由変数の取り扱い
            if (idx.Length == 0) return;
            var ary = Enumerable.Repeat(0.0, (int)idx.Max() + 1).ToArray();
            foreach (var item in idx)
            {
                ary[item] = 1;
                AddSubject(ary).GreaterThan(0);
                ary[item] = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Minimize(params double[] ary)
        {
            var maxlen = subs.Max(e => e.ary.Length);

            var target = new double[maxlen + subs.Count];
            for (var i = 0; i < ary.Length; ++i)
            {
                target[i] = ary[i];
            }
            var targetConstant = 0.0;
            var kitei = new long[subs.Count];
            var subjects = new double[subs.Count][];
            var constants = new double[subs.Count];
            for (var i = 0; i < subs.Count; ++i)
            {
                var sub = subs[i];
                kitei[i] = maxlen + i;
                subjects[i] = new double[maxlen + subs.Count];
                constants[i] = sub.value;
                for (var j = 0; j < sub.ary.Length; ++j)
                {
                    subjects[i][j] = sub.ary[j];
                }
                subjects[i][kitei[i]] = 1;
            }

            while (true)
            {
                var minidx = -1;
                var minvalue = 0.0;
                for (var i = 0; i < target.Length; ++i)
                {
                    if (minvalue > target[i])
                    {
                        minidx = i;
                        minvalue = target[i];
                    }
                }
                if (minidx == -1) break;

                minvalue = Double.PositiveInfinity;
                var selectedSubIdx = -1;
                for (var i = 0; i < subjects.Length; ++i)
                {
                    var zouka = constants[i] / subjects[i][minidx];
                    var neg1 = constants[i] < 0;
                    var neg2 = subjects[i][minidx] < 0;
                    if (neg1 && !neg2 || !neg1 && neg2) continue;
                    if (minvalue > zouka)
                    {
                        minvalue = zouka;
                        selectedSubIdx = i;
                    }
                }
                kitei[selectedSubIdx] = minidx;

                var selectedSub = subjects[selectedSubIdx];
                var mul = 1 / selectedSub[minidx];
                for (var i = 0; i < selectedSub.Length; ++i)
                {
                    selectedSub[i] *= mul;
                }
                constants[selectedSubIdx] *= mul;

                for (var i = 0; i < subjects.Length; ++i)
                {
                    if (i == selectedSubIdx) continue;
                    mul = subjects[i][minidx];
                    for (var j = 0; j < subjects[i].Length; ++j)
                    {
                        subjects[i][j] -= selectedSub[j] * mul;
                    }
                    constants[i] -= constants[selectedSubIdx] * mul;
                }
                mul = target[minidx];
                for (var i = 0; i < target.Length; ++i)
                {
                    target[i] -= selectedSub[i] * mul;
                }
                targetConstant -= constants[selectedSubIdx] * mul;
            }

            return -1 * targetConstant;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Maximize(params double[] ary)
        {
            ary = ary.ToArray();
            for (var i = 0; i < ary.Length; ++i) ary[i] *= -1;
            return -1 * Minimize(ary);
        }
    }
    ////end
}