using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AtCoder
{
    class Program
    {
        static void Main(string[] args)
        {
            string TARGET = "ABC125";

            var ts = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var item in ts)
            {
                var className = item.Name;
                if (className.StartsWith(TARGET))
                {
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
                                testCases[casename].Item1.AppendLine(line);
                            }
                            else if (nowOutput)
                            {
                                testCases[casename].Item2.AppendLine(line);
                            }
                        }
                    }
                    foreach (var kv in testCases)
                    {
                        var result = new StringWriter();
                        var oldIn = Console.In;
                        var oldOut = Console.Out;
                        Console.SetIn(new StringReader(kv.Value.Item1.ToString()));
                        Console.SetOut(result);
                        item.GetMethod("Main").Invoke(null, new object[] { new string[0] });
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
                                if (actual[i] != expect[i])
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }
                        Console.WriteLine(className + "." + kv.Key + (ok ? ": ok" : ": wrong"));
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
