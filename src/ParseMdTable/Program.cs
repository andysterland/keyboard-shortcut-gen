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
                    var commandString = firstCell.Trim();
                    int pos = 0;
                    while (!Char.IsLetter(firstCell[pos]))
                    {
                        ++pos;
                    }
                    var commandGroup = firstCell.Substring(pos, firstCell.LastIndexOf(".") + 1 - pos);
                    firstCell = firstCell.Replace(commandGroup, string.Empty);
                    firstCell = firstCell.Trim();
                    firstCell = ReplaceKnownIssues(firstCell);
                    firstCell = string.Join(string.Empty, PascalCaseToSentence(firstCell));
                    parts[1] = FirstCharToUpper(firstCell);
                    var outLine = string.Join("|", parts); 
                    outText.Append(outLine);
                    outText.Append($" {commandString} |");
                    outText.AppendLine();
                }
                else if(line.StartsWith("|Command", StringComparison.CurrentCultureIgnoreCase) || line.StartsWith("| Command", StringComparison.CurrentCultureIgnoreCase))
                {
                    outText.Append(line);
                    outText.Append($"Command ID|");
                    outText.AppendLine();

                }
                else if(line.StartsWith("|-") || line.Contains("| -"))
                {
                    outText.AppendLine("|-|-|-|");
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

        private static Dictionary<string, string> _issues = new Dictionary<string, string> {
            {"codeanalysis", "codea nalysis"},
            {"analysison", "analysis on"},
            {"Viewin", "View in"},
            {"Breakat", "Break at"},
            {"Attachto", "Attach to"},
            {"callor", "call or"},
            {"intelli trace", "IntelliTrace"},
            {"Intelli trace", "IntelliTrace"},
            {"Reattachto", "Reattach to"},
            {"Findin", "Find in"},
            {"Replacein", "Replace in"},
            {"editlabels", "edit labels"},
            {"Addand", "Add and"},
            {"Previewchanges", "Preview changes"},
            {"Publishselectedfiles", "Publish selected files"},
            {"Replaceselectedfilesfromserver", "Replace selected files from server"},
            {"tabto", "tab to"},
            {"Moveto", "Move to"},
            {"taband", "tab and"},
            {"addto", "add to"},
            {"Removefrom", "Remove from"},
            {"Bottomto", "Bottom to"},
            {"Leftto", "Left to"},
            {"Rightto", "Right to"},
            {"Topto", "Top to"},
            {"referenceto", "reference to"},
            {"Splitintoanewmethod", "Split into a new method"},
            {"Locatethe", "Locate the"},
            {"Convertto", "Convert to"},
            {"Columntothe", "Column to the"},
            {"focuson ", "focus on"},
            {"stackon ", "stack on"},
            {"Movecode ", "Move code"},
            {"issuein ", "issue in"},
            {"caretsat ", "carets at"},
            {"Sizeto ", "Size to"},
            {"Copyand", "Copy and"},
            {"Selectionto", "Selection to"},
            {"copyof", "copy of" }
        };
        private static string ReplaceKnownIssues(string input)
        {
            if (input == null) return "";

            foreach(var key in _issues.Keys)
            {
                if(input.Contains(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    return input.Replace(key, _issues[key], StringComparison.CurrentCultureIgnoreCase);
                }
            }

            return input;
        }

        private static string[] _acryonms = new string[]{"TFS", "SQL", "SSDT", "XSLT", "UML"};
        private static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: 
                    throw new ArgumentNullException(nameof(input));
                case "": 
                    throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default:
                    var lowerCase = input.First().ToString().ToUpper() + input.Substring(1);
                    foreach(var acroynm in _acryonms)
                    {
                        lowerCase = lowerCase.Replace(acroynm.ToLower(), acroynm.ToUpper());
                    }
                    return lowerCase;
            }
        }
    }
}
