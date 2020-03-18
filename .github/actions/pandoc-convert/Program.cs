using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using Arguments = CommandLine.Parser;

public class Program
{
    private static readonly string _courseNumber = "40575A";
    private static readonly string _courseTitle = "Power Platform Fundamentals";

    static void Main(string[] args) =>
        Arguments.Default.ParseArguments<Options>(args)
            .MapResult(options => {
                new Processor(_courseNumber, _courseTitle, options.Root);
                return 0;
            }, _ => 1);
}

internal class Options
{
    [Option('r', "root", Required = false, HelpText = "Root folder for content")]
    public string Root { get; set; }
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
        Root = root;
        TextInfo = new CultureInfo("en-US", false).TextInfo;
        FindMarkdownFiles();
    }

    private void FindMarkdownFiles()
    {
        Console.WriteLine("Finding Markdown Files");
        string root = Path.Combine(Directory.GetCurrentDirectory(), "src");
        var files = Directory.GetFiles(root, "*.md", SearchOption.AllDirectories);
        int count = 0;
        foreach(string file in files)
        {
            Console.WriteLine($"[{++count}]\t{file}");
        }
        CreatePackageDirectory();
        GetModules(CreateTempDirectory());
        var info = new ProcessStartInfo
        {
            FileName = "dir"
        };
        Process.Start(info).WaitForExit();
        var info2 = new ProcessStartInfo
        {
            FileName = "dir",
            WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "media")
        };
        Process.Start(info2).WaitForExit();
    }

    private void CreatePackageDirectory()
    {
        string pkgPath = Path.Combine(Directory.GetCurrentDirectory(), "pkg");
        Directory.CreateDirectory(pkgPath);
    }

    private string CreateTempDirectory()
    {
        string sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "src");
        string tempPath = sourcePath.Replace("src", "tmp");
        var info = new ProcessStartInfo
        {
            FileName = "cp",
            Arguments = $"-R {sourcePath} {tempPath}"
        };
        Process.Start(info).WaitForExit();
        return tempPath;
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
            WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "pkg"),         
            Arguments = $"{source} --from markdown --to docx --reference-doc={Path.Combine(Root, "template.docx")} --output \"{dest}\""
        };
        Process.Start(info).WaitForExit();
    }

    private void ProcessFileContent(string source)
    {
        string content = File.ReadAllText(source);

        content = Regex.Replace(content, "\\]\\(/media", $"]({Path.Combine(Root, "media")}");
        content = Regex.Replace(content, "icons\\/(.*)\\.png\\)", "icons/$1@3x.png){ height=\"0.4inch\" }");

        File.WriteAllText(
            source, 
            content
        );
    }
}