using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library
{
    ////start
    class LIB_GF
    {
        const long Mod = 998244353;
        public const int MaxDegree = (1 << 22) - 1;
        enum Kind { Constant, Variable, Polynomial, Add, Multiply, Divide, Power }
        readonly Kind kind;
        readonly long value;
        readonly string name;
        readonly LIB_GF left, right;
        readonly long[] polynomial;

        LIB_GF(Kind kind, long value = 0, string name = null, LIB_GF left = null, LIB_GF right = null, long[] polynomial = null)
        {
            this.kind = kind; this.value = value; this.name = name;
            this.left = left; this.right = right; this.polynomial = polynomial;
        }

        static long Normalize(long x)
        {
            x %= Mod;
            return x < 0 ? x + Mod : x;
        }
        static long ModPow(long a, long n)
        {
            long r = 1;
            for (; n > 0; n >>= 1, a = a * a % Mod) if ((n & 1) != 0) r = r * a % Mod;
            return r;
        }
        static long Inv(long a)
        {
            if (a == 0) throw new DivideByZeroException("The constant coefficient of a divisor must be nonzero.");
            return ModPow(a, Mod - 2);
        }
        bool Is(long c) => kind == Kind.Constant && value == c;

        public static LIB_GF Variable(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A variable name is required.", nameof(name));
            return new LIB_GF(Kind.Variable, name: name);
        }
        public static implicit operator LIB_GF(long value) => new LIB_GF(Kind.Constant, Normalize(value));
        public static LIB_GF operator +(LIB_GF a, LIB_GF b)
        {
            if (a.Is(0)) return b;
            if (b.Is(0)) return a;
            if (a.kind == Kind.Constant && b.kind == Kind.Constant) return a.value + b.value;
            return new LIB_GF(Kind.Add, left: a, right: b);
        }
        public static LIB_GF operator -(LIB_GF a) => -1 * a;
        public static LIB_GF operator -(LIB_GF a, LIB_GF b) => ReferenceEquals(a, b) ? (LIB_GF)0 : a + -b;
        public static LIB_GF operator *(LIB_GF a, LIB_GF b)
        {
            if (a.Is(0) || b.Is(0)) return 0;
            if (a.Is(1)) return b;
            if (b.Is(1)) return a;
            if (a.kind == Kind.Constant && b.kind == Kind.Constant) return a.value * b.value % Mod;
            return new LIB_GF(Kind.Multiply, left: a, right: b);
        }
        public static LIB_GF operator /(LIB_GF a, LIB_GF b)
        {
            if (b.Is(0)) throw new DivideByZeroException();
            if (b.kind == Kind.Constant) return a * Inv(b.value);
            return new LIB_GF(Kind.Divide, left: a, right: b);
        }
        public LIB_GF Pow(long exponent)
        {
            if (exponent < 0) throw new ArgumentOutOfRangeException(nameof(exponent));
            if (exponent == 0) return 1;
            if (exponent == 1) return this;
            if (kind == Kind.Constant) return ModPow(value, exponent);
            return new LIB_GF(Kind.Power, exponent, left: this);
        }

        static string VariableName(LIB_GF variable)
        {
            if (variable == null || variable.kind != Kind.Variable)
            {
                throw new ArgumentException("Pass a variable returned by Variable().", nameof(variable));
            }
            return variable.name;
        }
        bool Contains(string variable)
        {
            if (kind == Kind.Variable || kind == Kind.Polynomial) return name == variable;
            return (left != null && left.Contains(variable)) || (right != null && right.Contains(variable));
        }
        void Variables(HashSet<string> names)
        {
            if (kind == Kind.Variable || kind == Kind.Polynomial) names.Add(name);
            left?.Variables(names); right?.Variables(names);
        }
        static LIB_GF FromPolynomial(long[] p, string variable)
        {
            p = Trim(p);
            if (p.Length == 1) return p[0];
            return new LIB_GF(Kind.Polynomial, name: variable, polynomial: p);
        }

        public LIB_GF Coefficient(LIB_GF variable, long degree)
        {
            var x = VariableName(variable);
            if (degree < 0) throw new ArgumentOutOfRangeException(nameof(degree));
            var names = new HashSet<string>(); Variables(names); names.Remove(x);
            if (names.Count > 1) throw new NotSupportedException("Coefficient supports at most two variables.");
            var y = names.FirstOrDefault();
            return Extract(x, y, degree);
        }

        LIB_GF Extract(string x, string y, long n)
        {
            if (!Contains(x)) return n == 0 ? this : (LIB_GF)0;
            if (kind == Kind.Add) return left.Extract(x, y, n) + right.Extract(x, y, n);

            var numerators = new List<(LIB_GF expression, long multiplicity)>();
            var denominators = new List<(LIB_GF expression, long multiplicity)>();
            CollectFraction(this, x, false, 1, numerators, denominators);
            var numerator = new Dictionary<long, LIB_GF> { [0] = 1 };
            foreach (var term in numerators)
            {
                numerator = MultiplyInX(numerator, PolynomialInX(term.expression.Pow(term.multiplicity), x, n), n);
            }

            LIB_GF outsideDenominator = 1;
            var factors = new List<BinomialFactor>();
            var step = 0L;
            foreach (var term in denominators)
            {
                var factor = term.expression;
                if (!factor.Contains(x))
                {
                    outsideDenominator *= factor.Pow(term.multiplicity);
                    continue;
                }
                var parts = PolynomialInX(factor, x, long.MaxValue)
                    .Select(t => (degree: t.Key, coefficient: AsPolynomial(t.Value, y)))
                    .Where(t => !Zero(t.coefficient)).ToArray();
                var c = parts.FirstOrDefault(t => t.degree == 0).coefficient ?? new long[] { 0 };
                var inv = Inv(c[0]);
                outsideDenominator *= ((LIB_GF)c[0]).Pow(term.multiplicity);
                c = Scale(c, inv);
                var nonconstant = parts.Where(t => t.degree != 0).ToArray();
                if (nonconstant.Length == 0)
                {
                    outsideDenominator *= FromPolynomial(c, y).Pow(term.multiplicity);
                    continue;
                }
                if (nonconstant.Length != 1)
                {
                    throw new NotSupportedException("Each denominator factor must have the form c(y)-b(y)*x^d; automatic factorization is not supported.");
                }
                var power = nonconstant[0].degree;
                if (step != 0 && step != power)
                {
                    throw new NotSupportedException("All denominator factors must use the same power of the extracted variable.");
                }
                step = power;
                var b = Scale(nonconstant[0].coefficient, Normalize(-inv));
                var equal = factors.FirstOrDefault(f => f.Constant.SequenceEqual(c) && f.Linear.SequenceEqual(b));
                if (equal != null)
                {
                    equal.Multiplicity = AddMultiplicity(equal.Multiplicity, term.multiplicity);
                }
                else factors.Add(new BinomialFactor { Constant = c, Linear = b, Multiplicity = term.multiplicity });
            }
            if (factors.Count > 1 && factors.Any(f => f.Constant.Length != 1 || f.Multiplicity != 1))
            {
                throw new NotSupportedException("Multiple distinct factors require numeric constant terms and no repeated factors.");
            }

            var poles = factors.Select(f => f.Linear).ToList();
            var decomposition = factors.Count > 1 ? PartialFractions(poles) : null;
            LIB_GF result = 0;
            foreach (var term in numerator)
            {
                var index = n - term.Key;
                LIB_GF coefficient;
                if (factors.Count == 0) coefficient = index == 0 ? (LIB_GF)1 : (LIB_GF)0;
                else if (index % step != 0) continue;
                else if (factors.Count == 1)
                {
                    var q = index / step;
                    var r = factors[0].Multiplicity;
                    var b = FromPolynomial(factors[0].Linear, y);
                    var c = FromPolynomial(factors[0].Constant, y);
                    coefficient = MultisetCoefficient(q, r) * b.Pow(q) / (c.Pow(q) * c.Pow(r));
                }
                else coefficient = EvaluatePartialFractions(poles, decomposition, y, index / step);
                result += term.Value * coefficient;
            }
            return result / outsideDenominator;
        }

        class BinomialFactor
        {
            internal long[] Constant, Linear;
            internal long Multiplicity;
        }
        class Decomposition
        {
            internal long[] Denominator;
            internal List<long[]> Multipliers = new List<long[]>();
        }
        static long AddMultiplicity(long a, long b)
        {
            if (a > long.MaxValue - b) throw new NotSupportedException("The total factor multiplicity exceeds long.MaxValue.");
            return a + b;
        }
        static long MultisetCoefficient(long q, long r)
        {
            var k = Math.Min(q, r - 1);
            if (k > MaxDegree) throw new NotSupportedException("The binomial coefficient requires min(N/d, multiplicity-1) <= MaxDegree.");
            var start = Normalize(q % Mod + (r - 1) % Mod - k);
            var num = 1L;
            var den = 1L;
            for (var i = 1; i <= k; ++i)
            {
                num = num * ((start + i) % Mod) % Mod;
                den = den * i % Mod;
            }
            return num * Inv(den) % Mod;
        }

        static Decomposition PartialFractions(List<long[]> poles)
        {
            var ds = new List<long[]>(); var scales = new List<long>();
            long[] common = { 1 };
            for (var i = 0; i < poles.Count; ++i)
            {
                long[] d = { 1 };
                for (var j = 0; j < poles.Count; ++j)
                {
                    if (i == j) continue;
                    var difference = Add(poles[i], Scale(poles[j], Mod - 1));
                    if (Zero(difference)) throw new NotSupportedException("Repeated x-linear factors are not supported.");
                    if (difference[0] == 0) throw new NotSupportedException("Differences between poles must have nonzero constant coefficients; Laurent cancellation is not supported.");
                    d = PolyMultiply(d, difference);
                }
                var scale = Inv(d[0]);
                d = Scale(d, scale); ds.Add(d); scales.Add(scale);
                common = PolyMultiply(common, ExactDivide(d, Gcd(common, d)));
            }
            var result = new Decomposition { Denominator = common };
            for (var i = 0; i < poles.Count; ++i)
            {
                result.Multipliers.Add(Scale(ExactDivide(common, ds[i]), scales[i]));
            }
            return result;
        }
        static LIB_GF EvaluatePartialFractions(List<long[]> poles, Decomposition decomposition, string y, long n)
        {
            LIB_GF sum = 0;
            for (var i = 0; i < poles.Count; ++i)
            {
                var a = FromPolynomial(poles[i], y);
                var power = n <= long.MaxValue - (poles.Count - 1)
                    ? a.Pow(n + (poles.Count - 1)) : a.Pow(n) * a.Pow(poles.Count - 1);
                sum += FromPolynomial(decomposition.Multipliers[i], y) * power;
            }
            return sum / FromPolynomial(decomposition.Denominator, y);
        }

        static void CollectFraction(LIB_GF e, string x, bool invert, long multiplicity, List<(LIB_GF expression, long multiplicity)> num, List<(LIB_GF expression, long multiplicity)> den)
        {
            if (!e.Contains(x)) (invert ? den : num).Add((e, multiplicity));
            else if (e.kind == Kind.Multiply)
            {
                CollectFraction(e.left, x, invert, multiplicity, num, den);
                CollectFraction(e.right, x, invert, multiplicity, num, den);
            }
            else if (e.kind == Kind.Divide)
            {
                CollectFraction(e.left, x, invert, multiplicity, num, den);
                CollectFraction(e.right, x, !invert, multiplicity, num, den);
            }
            else if (e.kind == Kind.Power)
            {
                if (multiplicity > long.MaxValue / e.value)
                {
                    throw new NotSupportedException("The total factor multiplicity exceeds long.MaxValue.");
                }
                CollectFraction(e.left, x, invert, multiplicity * e.value, num, den);
            }
            else (invert ? den : num).Add((e, multiplicity));
        }

        static Dictionary<long, LIB_GF> PolynomialInX(LIB_GF e, string x, long limit)
        {
            if (!e.Contains(x)) return e.Is(0) ? new Dictionary<long, LIB_GF>() : new Dictionary<long, LIB_GF> { [0] = e };
            if (e.kind == Kind.Variable)
            {
                return limit == 0 ? new Dictionary<long, LIB_GF>() : new Dictionary<long, LIB_GF> { [1] = 1 };
            }
            if (e.kind == Kind.Polynomial)
            {
                var result = new Dictionary<long, LIB_GF>();
                for (var i = 0; i < e.polynomial.Length && i <= limit; ++i)
                {
                    if (e.polynomial[i] != 0) result[i] = e.polynomial[i];
                }
                return result;
            }
            if (e.kind == Kind.Add)
            {
                var result = PolynomialInX(e.left, x, limit);
                foreach (var term in PolynomialInX(e.right, x, limit)) AddTerm(result, term.Key, term.Value);
                return result;
            }
            if (e.kind == Kind.Multiply)
            {
                return MultiplyInX(PolynomialInX(e.left, x, limit), PolynomialInX(e.right, x, limit), limit);
            }
            if (e.kind == Kind.Divide && !e.right.Contains(x))
            {
                var result = PolynomialInX(e.left, x, limit);
                foreach (var key in result.Keys.ToArray()) result[key] /= e.right;
                return result;
            }
            if (e.kind == Kind.Power)
            {
                var a = PolynomialInX(e.left, x, limit);
                var result = new Dictionary<long, LIB_GF> { [0] = 1 };
                for (var n = e.value; n > 0; n >>= 1)
                {
                    if ((n & 1) != 0) result = MultiplyInX(result, a, limit);
                    if (n > 1) a = MultiplyInX(a, a, limit);
                }
                return result;
            }
            throw new NotSupportedException("A polynomial in the extracted variable is required; distribute sums of rational functions first.");
        }
        static void AddTerm(Dictionary<long, LIB_GF> terms, long degree, LIB_GF coefficient)
        {
            if (terms.TryGetValue(degree, out var old)) coefficient = old + coefficient;
            if (coefficient.Is(0)) terms.Remove(degree);
            else terms[degree] = coefficient;
        }
        static Dictionary<long, LIB_GF> MultiplyInX(Dictionary<long, LIB_GF> a, Dictionary<long, LIB_GF> b, long limit)
        {
            var result = new Dictionary<long, LIB_GF>();
            foreach (var first in a)
            {
                foreach (var second in b)
                {
                    if (first.Key > limit - second.Key) continue;
                    AddTerm(result, first.Key + second.Key, first.Value * second.Value);
                }
            }
            return result;
        }

        static long[] AsPolynomial(LIB_GF e, string variable)
        {
            switch (e.kind)
            {
                case Kind.Constant: return new[] { e.value };
                case Kind.Variable:
                    if (e.name != variable) throw new NotSupportedException("Unexpected variable.");
                    return new long[] { 0, 1 };
                case Kind.Polynomial:
                    if (e.name != variable) throw new NotSupportedException("Unexpected variable.");
                    return e.polynomial;
                case Kind.Add: return Add(AsPolynomial(e.left, variable), AsPolynomial(e.right, variable));
                case Kind.Multiply: return PolyMultiply(AsPolynomial(e.left, variable), AsPolynomial(e.right, variable));
                case Kind.Divide:
                    var divisor = AsPolynomial(e.right, variable);
                    if (divisor.Length != 1) throw new NotSupportedException("A polynomial coefficient is required.");
                    return Scale(AsPolynomial(e.left, variable), Inv(divisor[0]));
                case Kind.Power:
                    var a = AsPolynomial(e.left, variable);
                    if (a.Length == 1) return new[] { ModPow(a[0], e.value) };
                    if (e.value > MaxDegree / (a.Length - 1)) throw new NotSupportedException("An input polynomial is too large to expand.");
                    long[] r = { 1 };
                    for (var n = e.value; n > 0; n >>= 1)
                    {
                        if ((n & 1) != 0) r = PolyMultiply(r, a);
                        if (n > 1) a = PolyMultiply(a, a);
                    }
                    return r;
                default: throw new InvalidOperationException();
            }
        }
        static bool Zero(long[] a) => a.Length == 1 && a[0] == 0;
        static long[] Trim(long[] a)
        {
            var length = a.Length;
            while (length > 1 && a[length - 1] == 0) --length;
            if (length != a.Length) Array.Resize(ref a, length);
            return a;
        }
        static long[] Add(long[] a, long[] b)
        {
            var r = new long[Math.Max(a.Length, b.Length)];
            for (var i = 0; i < r.Length; ++i)
            {
                r[i] = ((i < a.Length ? a[i] : 0) + (i < b.Length ? b[i] : 0)) % Mod;
            }
            return Trim(r);
        }
        static long[] Scale(long[] a, long scalar)
        {
            if (scalar == 0) return new long[] { 0 };
            var r = new long[a.Length];
            for (var i = 0; i < r.Length; ++i)
            {
                r[i] = a[i] * scalar % Mod;
            }
            return Trim(r);
        }
        static long[] PolyMultiply(long[] a, long[] b)
        {
            if (Zero(a) || Zero(b)) return new long[] { 0 };
            if ((long)a.Length + b.Length - 2 > MaxDegree) throw new NotSupportedException("An intermediate polynomial is too large.");
            var r = new long[a.Length + b.Length - 1];
            for (var i = 0; i < a.Length; ++i)
            {
                for (var j = 0; j < b.Length; ++j)
                {
                    r[i + j] = (r[i + j] + a[i] * b[j]) % Mod;
                }
            }
            return Trim(r);
        }
        static (long[] quotient, long[] remainder) DivRem(long[] a, long[] b)
        {
            if (Zero(b)) throw new DivideByZeroException();
            var r = (long[])a.Clone(); var q = new long[Math.Max(1, a.Length - b.Length + 1)];
            var inv = Inv(b[b.Length - 1]);
            for (var i = a.Length - b.Length; i >= 0; --i)
            {
                q[i] = r[i + b.Length - 1] * inv % Mod;
                for (var j = 0; j < b.Length; ++j)
                {
                    r[i + j] = Normalize(r[i + j] - q[i] * b[j] % Mod);
                }
            }
            return (Trim(q), Trim(r));
        }
        static long[] ExactDivide(long[] a, long[] b)
        {
            var (q, r) = DivRem(a, b);
            if (!Zero(r)) throw new InvalidOperationException("Internal polynomial division was not exact.");
            return q;
        }
        static long[] Gcd(long[] a, long[] b)
        {
            while (!Zero(b))
            {
                var r = DivRem(a, b).remainder;
                a = b;
                b = r;
            }
            return Scale(a, Inv(a[0]));
        }

        public LIB_FPS ToFPS(LIB_GF variable, long maxDegree)
        {
            var y = VariableName(variable);
            if (maxDegree < 0 || maxDegree > MaxDegree) throw new ArgumentOutOfRangeException(nameof(maxDegree));
            var names = new HashSet<string>(); Variables(names);
            if (names.Any(n => n != y)) throw new ArgumentException("The expression still contains a different variable.", nameof(variable));
            var evaluator = new Evaluator((int)maxDegree);
            var coefficients = evaluator.Evaluate(this);
            return new LIB_FPS(maxDegree, coefficients.AsSpan());
        }

        class Evaluator
        {
            readonly int degree;
            long[] inverses;
            internal Evaluator(int degree) { this.degree = degree; }
            internal long[] Evaluate(LIB_GF e)
            {
                switch (e.kind)
                {
                    case Kind.Constant: return new[] { e.value };
                    case Kind.Variable: return degree == 0 ? new long[] { 0 } : new long[] { 0, 1 };
                    case Kind.Polynomial:
                        if (e.polynomial.Length <= degree + 1) return e.polynomial;
                        return Trim(e.polynomial.Take(degree + 1).ToArray());
                    case Kind.Add: return Add(Evaluate(e.left), Evaluate(e.right));
                    case Kind.Multiply: return Multiply(Evaluate(e.left), Evaluate(e.right));
                    case Kind.Divide: return Divide(Evaluate(e.left), Evaluate(e.right));
                    case Kind.Power: return Power(Evaluate(e.left), e.value);
                    default: throw new InvalidOperationException();
                }
            }
            long[] Multiply(long[] a, long[] b)
            {
                if (Zero(a) || Zero(b)) return new long[] { 0 };
                if (a.Length == 1) return Scale(b, a[0]);
                if (b.Length == 1) return Scale(a, b[0]);
                var length = Math.Min(degree + 1, a.Length + b.Length - 1);
                long[] r;
                if (Math.Min(a.Length, b.Length) <= 64)
                {
                    r = new long[length];
                    for (var i = 0; i < a.Length; ++i)
                    {
                        if (a[i] == 0) continue;
                        for (var j = 0; j < b.Length && i + j < length; ++j)
                        {
                            r[i + j] = (r[i + j] + a[i] * b[j]) % Mod;
                        }
                    }
                }
                else
                {
                    r = LIB_NTT.Multiply(a.AsSpan(), b.AsSpan());
                    if (r.Length > length) Array.Resize(ref r, length);
                }
                return Trim(r);
            }
            long[] Divide(long[] a, long[] b)
            {
                var inv = Inv(b[0]);
                if (Zero(a)) return new long[] { 0 };
                if (b.Length == 1) return Scale(a, inv);
                var terms = NonzeroTerms(b);
                if (terms.Count <= 64)
                {
                    var r = new long[degree + 1];
                    for (int n = 0; n <= degree; ++n)
                    {
                        var v = n < a.Length ? a[n] : 0;
                        foreach (var term in terms)
                        {
                            if (term.index > n) break;
                            v = Normalize(v - term.coefficient * r[n - term.index] % Mod);
                        }
                        r[n] = v * inv % Mod;
                    }
                    return Trim(r);
                }
                var inverse = new LIB_FPS(degree, b.AsSpan()).Inverse();
                return Multiply(a, ReadFPS(inverse, degree));
            }
            static List<(int index, long coefficient)> NonzeroTerms(long[] a)
            {
                var result = new List<(int index, long coefficient)>();
                for (var i = 1; i < a.Length; ++i)
                {
                    if (a[i] != 0) result.Add((i, a[i]));
                }
                return result;
            }
            static long[] ReadFPS(LIB_FPS f, int m)
            {
                var result = new long[m + 1];
                for (var i = 0; i <= m; ++i)
                {
                    result[i] = f[i];
                }
                return Trim(result);
            }
            long[] Power(long[] a, long exponent)
            {
                if (exponent == 0) return new long[] { 1 };
                if (exponent == 1) return a;
                if (Zero(a)) return new long[] { 0 };
                var valuation = 0;
                while (a[valuation] == 0) ++valuation;
                if (valuation > degree / exponent) return new long[] { 0 };
                var shift = (int)(valuation * exponent);
                var precision = degree - shift;
                var scalar = ModPow(a[valuation], exponent);
                var inv = Inv(a[valuation]);
                var unit = new long[Math.Min(a.Length - valuation, precision + 1)];
                for (var i = 0; i < unit.Length; ++i)
                {
                    unit[i] = a[i + valuation] * inv % Mod;
                }
                unit = Trim(unit);
                var n = exponent % Mod;
                long[] power;
                if (precision == 0 || unit.Length == 1 || n == 0) power = new long[] { 1 };
                else
                {
                    var terms = NonzeroTerms(unit);
                    if (terms.Count <= 64)
                    {
                        EnsureInverses();
                        power = new long[precision + 1]; power[0] = 1;
                        for (var k = 1; k <= precision; ++k)
                        {
                            var v = 0L;
                            foreach (var term in terms)
                            {
                                if (term.index > k) break;
                                var factor = Normalize((n + 1) * term.index - k);
                                v = (v + term.coefficient * power[k - term.index] % Mod * factor) % Mod;
                            }
                            power[k] = v * inverses[k] % Mod;
                        }
                        power = Trim(power);
                    }
                    else
                    {
                        var fps = new LIB_FPS(precision, unit.AsSpan()).Pow(n);
                        power = ReadFPS(fps, precision);
                    }
                }
                var result = new long[shift + power.Length];
                for (var i = 0; i < power.Length; ++i)
                {
                    result[i + shift] = power[i] * scalar % Mod;
                }
                return Trim(result);
            }
            void EnsureInverses()
            {
                if (inverses != null) return;
                inverses = new long[degree + 1];
                if (degree >= 1) inverses[1] = 1;
                for (var i = 2; i <= degree; ++i) inverses[i] = Mod - (Mod / i) * inverses[Mod % i] % Mod;
            }
        }

        public override string ToString()
        {
            switch (kind)
            {
                case Kind.Constant: return Signed(value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case Kind.Variable: return name;
                case Kind.Polynomial: return PolynomialString(polynomial, name);
                case Kind.Add: return "(" + left + " + " + right + ")";
                case Kind.Multiply: return "(" + left + " * " + right + ")";
                case Kind.Divide: return "(" + left + " / " + right + ")";
                case Kind.Power: return "(" + left + ")^" + value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                default: throw new InvalidOperationException();
            }
        }
        static long Signed(long v) => v > Mod / 2 ? v - Mod : v;
        static string PolynomialString(long[] p, string variable)
        {
            var result = new StringBuilder();
            for (int i = 0; i < p.Length; ++i)
            {
                if (p[i] == 0) continue;
                var c = Signed(p[i]);
                if (result.Length > 0) result.Append(c < 0 ? " - " : " + ");
                else if (c < 0) result.Append('-');
                c = Math.Abs(c);
                if (i == 0 || c != 1) result.Append(c.ToString(System.Globalization.CultureInfo.InvariantCulture));
                if (i > 0)
                {
                    if (c != 1) result.Append('*');
                    result.Append(variable);
                    if (i != 1) result.Append('^').Append(i);
                }
            }
            return result.Length == 0 ? "0" : "(" + result + ")";
        }
    }
    ////end
}
