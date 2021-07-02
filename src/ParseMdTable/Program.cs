using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseMdTable
{
    class Program
    {
        public static int Main(params string[] args)
        {
            Command cmd = new Command("root")
            {
                new Option("--source", "The file containing the source content.", typeof(string)),
                new Option("--target", "The file containing the target content.", typeof(string))
            };
            cmd.Description = "Parse the Markdown file with keyboard shortcuts.";
            cmd.Handler = CommandHandler.Create<string, string, IConsole>(Handler);
            return cmd.Invoke(args);
        }

        public static void Handler(string source, string target, IConsole console)
        {
            console.Out.Write($"Processing {source} to {target}\n");
            if (!string.IsNullOrEmpty(source) && !File.Exists(source))
            {
                console.Error.Write($"Not found: {source}");
                throw new ArgumentNullException($"--source not found {source}\n");
            }

            var outText = new StringBuilder();
            var sourceText = File.ReadAllLines(source);
            foreach (var line in sourceText)
            {
                if (line.Contains(".") && line.Count(ch => ch == '|') == 3)
                {
                    var parts = line.Split("|");
                    var firstCell = parts[1];
                    int pos = 0;
                    while (!Char.IsLetter(firstCell[pos]))
                    {
                        ++pos;
                    }
                    var commandGroup = firstCell.Substring(pos, firstCell.LastIndexOf(".") + 1 - pos);
                    firstCell = firstCell.Replace(commandGroup, string.Empty);
                    firstCell = firstCell.Trim();
                    firstCell = string.Join("", PascalCaseToSentence(firstCell));
                    parts[1] = FirstCharToUpper(firstCell);
                    var outLine = string.Join("|", parts);
                    outText.Append(outLine);
                    outText.AppendLine();
                }
                else
                {
                    outText.AppendLine(line);
                }
            }

            console.Out.Write($"Writting {outText.Length} chars to {target}\n");
            File.WriteAllText(target, outText.ToString());
        }
        private static string PascalCaseToSentence(string input)
        {
            if (input == null) return "";

            string output = Regex.Replace(input, @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])", m => " " + m.Value);
            output = output.ToLower();
            return output;
        }

        private static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}
