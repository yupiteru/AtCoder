using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AtCoder
{
    class Driver
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "direct")
            {
                Program.ProblemA.Main(args);
                return;
            }
            var ts = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var item in ts)
            {
                if (!item.IsClass || item.Namespace != "Program" || item.GetMethods().All(e => e.Name != "Main")) continue;

                var className = item.Name;
                var testCaseFile = Directory.GetFiles(".", className + "_TestCase.txt", System.IO.SearchOption.AllDirectories);
                if (testCaseFile.Length == 0)
                {
                    Console.WriteLine(className + ": testcase file is not found");
                    continue;
                }
                else if (testCaseFile.Length != 1)
                {
                    Console.WriteLine(className + ": testcase files are found two or more. use only first testcase file");
                }

                var testCases = new Dictionary<string, Tuple<StringBuilder, StringBuilder>>();
                var casename = "";
                var nowInput = false;
                var nowOutput = false;
                foreach (var line in File.ReadAllLines(testCaseFile[0]))
                {
                    if (line.StartsWith("[input]"))
                    {
                        nowInput = true;
                        nowOutput = false;
                    }
                    else if (line.StartsWith("[output]"))
                    {
                        nowInput = false;
                        nowOutput = true;
                    }
                    else if (line.StartsWith("["))
                    {
                        casename = line.Substring(1, line.Length - 2);
                        testCases[casename] = Tuple.Create(new StringBuilder(), new StringBuilder());
                        nowInput = true;
                        nowOutput = false;
                    }
                    else
                    {
                        if (nowInput)
                        {
                            testCases[casename].Item1.AppendLine(line.Trim());
                        }
                        else if (nowOutput)
                        {
                            testCases[casename].Item2.AppendLine(line.Trim());
                        }
                    }
                }
                var outputStrings = new List<string>();
                var isValidProgram = false;
                foreach (var kv in testCases)
                {
                    var result = new StringWriter();
                    var oldIn = Console.In;
                    var oldOut = Console.Out;
                    Console.SetIn(new StringReader(kv.Value.Item1.ToString()));
                    Console.SetOut(result);
                    item.GetMethod("Main").Invoke(null, new object[] { new string[] { "debug" } });
                    Console.SetIn(oldIn);
                    Console.SetOut(oldOut);

                    var actual = result.GetStringBuilder().ToString().Split("\r\n".ToCharArray());
                    var expect = kv.Value.Item2.ToString().Split("\r\n".ToCharArray());
                    var ok = true;
                    for (var i = 0; i < Math.Max(actual.Length, expect.Length); ++i)
                    {
                        if (i >= actual.Length)
                        {
                            if (expect[i].Trim() != "")
                            {
                                ok = false;
                                break;
                            }
                        }
                        else if (i >= expect.Length)
                        {
                            if (actual[i].Trim() != "")
                            {
                                ok = false;
                                break;
                            }
                        }
                        else
                        {
                            if (actual[i].Trim() != expect[i].Trim())
                            {
                                ok = false;
                                break;
                            }
                        }
                    }
                    outputStrings.Add(className + "." + kv.Key + (ok ? ": ok" : ": wrong"));
                    if (!ok)
                    {
                        foreach (var item2 in expect)
                        {
                            if (item2 == "") continue;
                            outputStrings.Add("    expected: " + item2);
                        }
                        foreach (var item2 in actual)
                        {
                            if (item2 == "") continue;
                            outputStrings.Add("    actually: " + item2);
                        }
                    }
                    if (actual.Length > 1 || actual[0] != "") isValidProgram = true;
                }
                if (!isValidProgram) continue;
                outputStrings.ForEach(Console.WriteLine);
                if (item.GetField("numberOfRandomCases") != null && item.GetMethod("MakeTestCase") != null)
                {
                    var numberOfRandomCases = (int)item.GetField("numberOfRandomCases").GetValue(null);
                    if (numberOfRandomCases > 0)
                    {
                        Console.WriteLine($"{className} Random Case");
                        var doneCount = 0;
                        while (--numberOfRandomCases >= 0)
                        {
                            var input = new List<string>();
                            var output = new List<string>();
                            var param = new object[] { input, output, null };
                            item.GetMethod("MakeTestCase").Invoke(null, param);

                            var result = new StringWriter();
                            var oldIn = Console.In;
                            var oldOut = Console.Out;
                            Console.SetIn(new StringReader(string.Join("\r\n", input)));
                            Console.SetOut(result);
                            item.GetMethod("Main").Invoke(null, new object[] { new string[] { "debug" } });
                            Console.SetIn(oldIn);
                            Console.SetOut(oldOut);

                            var actual = result.GetStringBuilder().ToString().Split("\n".ToCharArray()).Select(e => e.Trim()).ToArray();
                            var expect = output.ToArray();
                            var checker = (Func<string[], bool>)param[2];
                            var ok = true;
                            if (checker == null)
                            {
                                for (var i = 0; i < Math.Max(actual.Length, expect.Length); ++i)
                                {
                                    if (i >= actual.Length)
                                    {
                                        if (expect[i].Trim() != "") { ok = false; break; }
                                    }
                                    else if (i >= expect.Length)
                                    {
                                        if (actual[i].Trim() != "") { ok = false; break; }
                                    }
                                    else
                                    {
                                        if (actual[i].Trim() != expect[i].Trim()) { ok = false; break; }
                                    }
                                }
                            }
                            else
                            {
                                ok = checker(actual);
                            }
                            Console.Write($"\r{++doneCount} cases done.");
                            if (!ok)
                            {
                                Console.WriteLine("");
                                foreach (var item2 in input)
                                {
                                    if (item2 == "") continue;
                                    Console.WriteLine("    input: " + item2);
                                }
                                foreach (var item2 in expect)
                                {
                                    if (item2 == "") continue;
                                    Console.WriteLine("    expected: " + item2);
                                }
                                foreach (var item2 in actual)
                                {
                                    if (item2 == "") continue;
                                    Console.WriteLine("    actually: " + item2);
                                }
                            }
                        }
                        Console.WriteLine("");
                    }
                }
            }
            Console.WriteLine("End.");
            Console.ReadLine();
        }
    }
}
