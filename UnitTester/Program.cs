using System;
using System.IO;
using System.Linq;

namespace UnitTester
{
    public class Program
    {
        static void Main(string[] args)
        {
            string path = "C:\\Users\\TheBoss\\source\\repos\\BulkUploader\\tester1.csv";

            string test = File.ReadAllText(path);
            Console.WriteLine(CheckLineEnding(test));
            Console.ReadLine();
        }

        public static bool CheckLineEnding(string text)
        {
            return text.Contains("r\n") || (text.Contains('\r') && !text.Contains("\n"));
        }
    }
}
