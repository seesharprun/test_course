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
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.md", SearchOption.AllDirectories);
            foreach(string file in files)
            {
                Console.WriteLine($"[{++count}]\t{file}");
            }
        }
    }
}