﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
public class Program
{
    private static readonly string _courseNumber = "40575A";
    private static readonly string _courseTitle = "Power Platform Fundamentals";

    static void Main(string[] args)
    {
        Console.WriteLine("Finding Markdown Files");
        string root = Path.Combine(Directory.GetCurrentDirectory(), "src");
        var files = Directory.GetFiles(root, "*.md", SearchOption.AllDirectories);
        int count = 0;
        foreach(string file in files)
        {
            Console.WriteLine($"[{++count}]\t{file}");
        }
        new Processor(_courseNumber, _courseTitle, root);
    }
}

internal class Processor
{
    public string Number { get; }
    public string Title { get; }
    public string Root { get; }
    public TextInfo TextInfo { get; }

    public Processor(string number, string title, string root)
    {
        Number = number;
        Title = title;
        TextInfo = new CultureInfo("en-US", false).TextInfo;
        CreateTempDirectory(root);
    }

    private void CreateTempDirectory(string sourcePath)
    {
        string tempPath = sourcePath.Replace("src", "tmp");
        var info = new ProcessStartInfo
        {
            FileName = "xcopy",
            Arguments = $"{sourcePath} {tempPath} /f /i /e "
        };
        Process.Start(info).WaitForExit();
        GetModules(tempPath);
    }

    private void GetModules(string coursePath)
    {
        var paths = Directory.GetDirectories(coursePath, "module_*", SearchOption.TopDirectoryOnly);
        foreach(string path in paths)
        {
            string moduleString = Regex.Match(path, "module_([0-9]{1,2})").Groups[1].Value;
            int module = Int32.Parse(moduleString);
            ProcessModule(path, module);
            GetLessons(path, module);
        }
    }

    private void GetLessons(string modulePath, int module)
    {
        var paths = Directory.GetDirectories(modulePath, "lesson_*", SearchOption.TopDirectoryOnly);
        foreach(string path in paths)
        {
            string lessonString = Regex.Match(path, "lesson_([0-9]{1,2})").Groups[1].Value;
            int lesson = Int32.Parse(lessonString);
            ProcessLesson(path, module, lesson);
        }
    }

    private void ProcessModule(string path, int module)
    {
        Console.WriteLine($"Processing Module {module:0}");
        var files = Directory.GetFiles(path, "*.md", SearchOption.TopDirectoryOnly);
        foreach(string file in files)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            string document = TextInfo.ToTitleCase(filename.Replace('_', ' '));
            string newFilename = $"{Number} {Title} Module {module} {document}.docx";
            string title = $"module_{module:00}\\{Path.GetFileName(file)}\t\t->\t{newFilename}";
            Console.WriteLine($"{title}");
            ConvertToMarkdown(file, newFilename);
        }
    }

    private void ProcessLesson(string path, int module, int lesson)
    {
        var files = Directory.GetFiles(path, "*.md", SearchOption.TopDirectoryOnly);
        foreach(string file in files)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            string document = TextInfo.ToTitleCase(filename.Replace('_', ' '));
            string newFilename = $"{Number} {Title} Module {module} Lesson {lesson} {document}.docx";
            string title = $"module_{module:00}\\lesson_{lesson:00}\\{Path.GetFileName(file)}\t->\t{newFilename}";
            Console.WriteLine($"{title}");
            ConvertToMarkdown(file, newFilename);
        }
    }

    private void ConvertToMarkdown(string source, string dest)
    {     
        ProcessFileContent(source);
        var info = new ProcessStartInfo
        {
            FileName = "pandoc",
            Arguments = $"{source} --from markdown --to docx --reference-doc=template.docx --output \"{dest}\""
        };
        Process.Start(info).WaitForExit();
    }

    private void ProcessFileContent(string source)
    {
        string content = File.ReadAllText(source);

        content = Regex.Replace(content, "\\]\\(/media/", "](media/");
        content = Regex.Replace(content, "icons\\/(.*)\\.png\\)", "icons/$1@3x.png){ height=\"0.4inch\" }");

        File.WriteAllText(
            source, 
            content
        );
    }
}