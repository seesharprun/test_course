using System;
using System.IO;
using Console = Colorful.Console;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteAscii("Finding Markdown Files");
            int count = 0;
            foreach(string markdownFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.md"))
            {
                Console.WriteLine($"[{++count}]\t{markdownFile}");
            }
        }
    }
}