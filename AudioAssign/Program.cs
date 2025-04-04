using Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAssign
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string outputDir = args.Length > 0 ? args[0] : "Recordings"; 
            var recorder = new Audio(outputDir); 
            recorder.Start();
            Console.WriteLine("Press 'x' to exit"); 
            while (Console.ReadKey(true).Key != ConsoleKey.X) { }
            recorder.Stop();
        }
    }
}
