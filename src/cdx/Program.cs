using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace dirx
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var context = new DirContext(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);

            Render(context, context.ActiveDir);

            var key = Console.ReadKey();
            while (key.Key != ConsoleKey.Enter)
            {
                var prevPrint = Path.Combine(context.ActiveDir, context.TargetFolder);

                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;

                    case ConsoleKey.LeftArrow:
                        context.MoveBack();
                        break;

                    case ConsoleKey.UpArrow:
                        context.SelectPrev();
                        break;

                    case ConsoleKey.RightArrow:
                        context.MoveForward();
                        break;

                    case ConsoleKey.DownArrow:
                        context.SelectNext();
                        break;

                    case ConsoleKey.Backspace:
                        if (context.SearchTerm.Length <= 0)
                        {
                            context.MoveBack();
                        }
                        else
                        {
                            context.SetSearchTerm(context.SearchTerm.Substring(0, Math.Max(0, context.SearchTerm.Length - 1)));
                        }
                        break;

                    default:
                        if ("1234567890 abcdefghijklmnopqrstuvwxyz".Contains(key.KeyChar.ToString().ToLower()))
                        {
                            context.SetSearchTerm($"{context.SearchTerm}{key.KeyChar}");
                        }
                        break;
                }

                Render(context, prevPrint);

                key = Console.ReadKey(true);
            }

            SetPath(context.ActiveDir);

            Console.ResetColor();
            Environment.Exit(0);
        }

        private static void SetPath(string dir)
        {
            var curProcess = Process.GetCurrentProcess();
            var programPath = Directory.GetParent(curProcess.MainModule.FileName).FullName;

            var batFileContents = $"@echo off\ncd /d \"{dir}\"\n";
            var batFilePath = Path.Combine(programPath, "dest.bat");
            File.WriteAllText(batFilePath, batFileContents);
        }

        private static void Render(DirContext context, string prevPrint)
        {
            var curPrint = new StringBuilder();
            Console.CursorLeft = 0;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(context.ActiveDir);
            curPrint.Append(context.ActiveDir);
            Console.Write(Path.DirectorySeparatorChar);
            curPrint.Append(Path.DirectorySeparatorChar);

            if (context.SearchTerm.Length > 0 && string.IsNullOrEmpty(context.TargetFolder))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.Write(context.SearchTerm);
            curPrint.Append(context.SearchTerm);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(context.TargetFolder.Substring(Math.Min(context.TargetFolder.Length, context.SearchTerm.Length)));
            curPrint.Append(context.TargetFolder.Substring(Math.Min(context.TargetFolder.Length, context.SearchTerm.Length)));

            Console.Write(new string(' ', Math.Max(curPrint.Length, prevPrint.Length)));

            Console.CursorLeft = context.ActiveDir.Length;
            Console.ResetColor();
        }
    }
}
