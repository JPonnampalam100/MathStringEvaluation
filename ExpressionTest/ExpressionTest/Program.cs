using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpressionEvaluator;

namespace ExpressionTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var data = ReadTestData("Math.txt");
            ExecuteTests(data);
        }

        private static void ExecuteTests(List<TestData> data)
        {
            var errCount = 0;
            foreach (var test in data)
            {
                var result = ExpressionEvaluator.ExpressionEvaluator.Calculate(test.Expression);
                if (Math.Round(result, 1) != Math.Round(test.ExpectedValue, 1)) errCount++;
            }

            Console.WriteLine($"Errors={errCount}");
        }

        private static List<TestData> ReadTestData(string mathTxtFile)
        {
            var lines = File.ReadAllLines(mathTxtFile).Where(l => !string.IsNullOrEmpty(l));
            return lines.Select(line => line.Split("|", StringSplitOptions.RemoveEmptyEntries)).Select(components =>
                new TestData {ExpectedValue = double.Parse(components[1].Trim()), Expression = components[0]}).ToList();
        }
    }

    public class TestData
    {
        public string Expression { get; set; }

        public double ExpectedValue { get; set; }
    }
}