using NodeSystem.CliSystem.Data;
using NodeSystem.CliSystem.Extensions;
using NodeSystem.Commands;
using NodeSystem.Commands.Base;
using NodeSystem.Commands.Data;

namespace NodeSystem.CliSystem.Core
{
    public static class CliProcessor
    {
        private static readonly List<string> noneArguments = new() { "?" };
        private static int savedInputIndex = -1;
        private static readonly List<string> savedInputs = new List<string>();
        private static HashSet<CliKeyword>? _keywords = null;
        private const string PROMPT = "CMD>";
        private static int cursorPositionX = 0; // Track cursor position for arrow key navigation
        public static string TypedText { get; private set; } = string.Empty;


        public static HashSet<CliKeyword> Keywords
        {
            get
            {
                if(_keywords == null)
                {
                    _keywords = GetKeywords(CliHandler.Commands);
                }
                return _keywords;
            }
        }

        private static bool ValidateChar(char c)
        {
            if (char.IsDigit(c) || char.IsLetter(c) || char.IsNumber(c) || c == '-' || noneArguments.Contains(c.ToString()) || c == '\r' || c == '\b' || c == ' ')
            {
                return true;
            }

            return false;
        }
        public static string ProcessInputs()
        {
            cursorPositionX = 0;
            TypedText = string.Empty;
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(PROMPT); // Display the initial prompt

            int currentLine = Console.CursorTop; // Get the current line

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true); // Read key without displaying it

                if (keyInfo.Key == ConsoleKey.Tab)
                {
                    var words = TypedText.Split(' ');
                    if (words.Length == 0 || words[0] == string.Empty)
                        continue;
                    var baseKeywords = Keywords.Where(x => x.CommandName != null && x.CommandName.Contains(words[0]));

                    if(baseKeywords.Any() && words.Length > 0)
                    {
                        string lastWord = words[words.Length - 1].Trim();
                        foreach (var item in baseKeywords)
                        {
                            if (item.Name != lastWord && lastWord != string.Empty && item.Name.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase))
                            {
                                words[words.Length - 1] = item.Name; // Replace the last word with the suggestion
                                TypedText = string.Join(' ', words) + " ";
                                cursorPositionX = TypedText.Length;

                                ResetCmdText();
                                HighlightKeywords(TypedText, Keywords, cursorPositionX);
                                break;
                            }
                        }
                    }
                                      
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    // Handle history navigation (Up arrow)
                    if (savedInputIndex > 0)
                    {
                        savedInputIndex--;
                        TypedText = savedInputs[savedInputIndex];
                    }
                    else
                    {
                        savedInputIndex = -1;
                        TypedText = string.Empty;
                    }

                    cursorPositionX = TypedText.Length;
                    ResetCmdText();
                    HighlightKeywords(TypedText, Keywords, cursorPositionX);
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (savedInputIndex + 1 < savedInputs.Count)
                    {
                        savedInputIndex++;
                        TypedText = savedInputs[savedInputIndex];
                        cursorPositionX = TypedText.Length;

                        ResetCmdText();
                        HighlightKeywords(TypedText, Keywords, cursorPositionX);
                    }               
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    // Move cursor left
                    if (cursorPositionX > 0)
                    {
                        cursorPositionX--;
                        Console.SetCursorPosition(cursorPositionX, Math.Max(0, Console.CursorTop)); 
                    }
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    // Move cursor right
                    if (cursorPositionX < TypedText.Length)
                    {
                        cursorPositionX++;
                        Console.SetCursorPosition(cursorPositionX, Math.Max(0, Console.CursorTop)); 
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    // Handle Enter key: finalize input
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;

                    if(TypedText.Length > 0)
                    {
                        savedInputs.Add(TypedText);
                        savedInputIndex = savedInputs.Count;
                    }
             

                    return TypedText;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    // Handle Backspace: remove character before the cursor
                    if (cursorPositionX > 0)
                    {
                        TypedText = TypedText.Remove(cursorPositionX - 1, 1);
                        cursorPositionX--;

                        Console.SetCursorPosition(PROMPT.Length, currentLine); // Move to the start of the current line
                        Console.Write(new string(' ', Console.LargestWindowWidth * 5)); // Clear the line (leave 1 char space to avoid wrap)
                        Console.SetCursorPosition(0, currentLine); // Reset cursor to the start of t
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(PROMPT); // Clear the line (leave 1 char space to avoid wrap)

                        HighlightKeywords(TypedText, Keywords, cursorPositionX);
                    }
                }
                else
                {
                    if (TypedText.Length > Console.BufferWidth - 8)
                        continue;
                    // Handle regular character input
                    char inputChar = keyInfo.KeyChar;
                    if (ValidateChar(inputChar))
                    {
                        TypedText = TypedText.Insert(cursorPositionX, inputChar.ToString());
                        cursorPositionX++;
                        ResetCmdText();

                        HighlightKeywords(TypedText, Keywords, cursorPositionX);
                    }
                }
            }

        }

        private static void ResetCmdText()
        {
            Console.Write("\r" + new string(' ', Console.BufferWidth)); // Clear current line
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\r" + PROMPT); // Write the prompt
        }

        private static HashSet<CliKeyword> GetKeywords(List<CliCommand> commands)
        {
            HashSet<CliKeyword> keywords = new();
            foreach (var item in commands)
            {
                keywords.Add(new CliKeyword { Name = item.Name.ToLower(), CommandName = item.Name, type = CliKeywordType.BASE });
                if(item.Cmd != null)
                {
                    var arguments = item.Cmd.Arguments?.Select(arg => arg.Name).ToList();
                    if (arguments != null)
                    {
                        arguments.ForEach(x =>
                        {
                            keywords.Add(new CliKeyword { Name = $"--{x}", CommandName = item.Name, type = CliKeywordType.VARIABLE });
                        });

                    }
                }
                if(item.SubCommands != null)
                {
                    foreach (var subCmd in item.SubCommands)
                    {
                        subCmd.Arguments?.ForEach(x =>
                        {
                            keywords.Add(new CliKeyword { Name = $"--{x.Name}", CommandName = item.Name, type = CliKeywordType.VARIABLE });
                        });
                    }
                    
                }
            }

            noneArguments.ForEach(x =>
            {
                keywords.Add(new CliKeyword { Name = x, type = CliKeywordType.NONE });
            });

            return keywords;
        }

        private static void HighlightKeywords(string input, HashSet<CliKeyword> keywords, int cursorPosition)
        {
            // Split the input into words and track positions
            var words = input.Split(new[] { ' ' }, StringSplitOptions.None);
            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i];
                var baseKeywords = keywords.Where(x => x.CommandName == words[0]);

                var foundKeyword = baseKeywords.FirstOrDefault(x => x.Name.ToLower().Equals(word));
                if (foundKeyword != null)
                {
                    if (foundKeyword.type == CliKeywordType.VARIABLE)
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    else
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.Write(word);
                }
                else
                {
                    if(noneArguments.Contains(word))
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Yellow;

                    // Regular text
                    Console.Write(word);
                }

                // Add a space only between words, not after the last word
                if (i < words.Length - 1)
                    Console.Write(" ");
            }

            // Reset color at the end
            Console.ResetColor();

        }

        public static (Dictionary<string, string>?, List<ICommand>?) ExtractCommands(string cmdText)
        {
            if (cmdText.Trim().Length == 0)
                return (null, null);
            List<ICommand> result = new();

            var allArgs = cmdText.ToLower().Trim().ExtractAnyArguments(noneArguments);
            var variableArgsMap = allArgs.Where(x => x.ArgType == ArgumentType.VARIABLE).ToDictionary(
                                kvp => kvp.Key.Replace("--", string.Empty), // Transform the key
                                kvp => kvp.Value          // Keep the value as is
                            );
            var baseArgsMap = allArgs.Where(x => x.ArgType == ArgumentType.BASE).ToDictionary(
                                kvp => kvp.Key.Replace("--", string.Empty), // Transform the key
                                kvp => kvp.Value          // Keep the value as is
                            );
            var namesArgsMap = allArgs.Where(x => x.ArgType == ArgumentType.NAME).ToDictionary(
                               kvp => kvp.Key.Replace("--", string.Empty), // Transform the key
                               kvp => kvp.Value          // Keep the value as is
                           );
            string baseName = cmdText.Substring(0, cmdText.IndexOf(" ") == -1 ? cmdText.Length : cmdText.IndexOf(" ")).Trim();

            List<string> inputs = new()
            {
                baseName
            };

            if (allArgs != null && allArgs.Count > 0)
            {
                inputs.AddRange(variableArgsMap.Keys);
                for (int i = 0; i < inputs.Count; i++)
                {
                    inputs[i] = inputs[i].Replace("--", string.Empty);
                }
            }

            var foundCommands = CliHandler.GetCommands(inputs, CliHandler.Commands);
            if (foundCommands != null)
            {
                foreach (var item in foundCommands)
                {
                    result.Add(item);
                }
            }

            if (!ValidateNamesArguments(namesArgsMap, baseName))
            {
                return (null, null);
            }

            if (!ValidateVariableArguments(foundCommands, variableArgsMap))
            {
                return (null, null);
            }

            Dictionary<string, string> processedArgs = new();

            foreach (var arg in variableArgsMap)
            {
                foreach (var item in result)
                {
                    string argKey = arg.Key.Replace("--", "");
                    var foundArg = item.Arguments?.SingleOrDefault(x => x.Name.ToLower() == argKey);

                    if (foundArg != null)
                    {
                        string? value = foundArg!.StaticValue ?? arg.Value ?? foundArg!.Default;

                        if (value == null)
                        {
                            item.ValidateArgs(variableArgsMap);
                            return (null, null);
                        }

                        processedArgs.Add(arg.Key, value!);

                        break;
                    }
                }
            }

            if (baseArgsMap.Any() && baseArgsMap.First().Value != null && !noneArguments.Contains(baseArgsMap.First().Value) && !variableArgsMap.Any())
            {
                Console.WriteLine($"{cmdText} is not recognized as command.");
                return (null, null);
            }

            if (!baseArgsMap.Any() && !namesArgsMap.Any() && !variableArgsMap.Any() && !foundCommands.Any())
            {
                Console.WriteLine($"{cmdText} is not recognized as command.");
                return (null, null);
            }
            if (namesArgsMap.Count > 0)
            {
                var cmdByNames = CliHandler.GetSubCommandsBaseName(baseName, namesArgsMap.Keys.ToList());

                foreach (var cmd in cmdByNames)
                {
                    cmd.DisplayArgsInfo();
                }
            }
            else if (baseArgsMap.Any() && noneArguments.Contains(baseArgsMap.First().Value))
            {
                var cmdByNames = CliHandler.GetCommandsByBaseName(baseName);

                foreach (var cmd in cmdByNames)
                {
                    cmd.DisplayArgsInfo();
                }
                if (!cmdByNames.Any())
                {
                    var baseCmd = CliHandler.Commands.FirstOrDefault(x => x.Cmd != null && x.Cmd.Name.ToLower() == baseName);
                    if (baseCmd != null)
                    {
                        baseCmd.Cmd.DisplayArgsInfo();
                    }else
                    Console.WriteLine($"{cmdText} is not recognized as command.");
                }

                return (null, null);
            }
            bool foundArgsInfo = false;
            foreach (var v in variableArgsMap)
            {
                if(v.Value == "?")
                {
                    var cmdByNames = CliHandler.GetCommandsByBaseName(baseName);
                    if(cmdByNames != null)
                    {
                        var args = cmdByNames.SelectMany(x => x.Arguments ?? new List<CliArgument>()).Where(x => x.Name == v.Key.Replace("--", string.Empty)).Distinct();
                        DisplayArgumentsInfo(args.ToArray());
                        foundArgsInfo = true;
                    }
                }
            }

            if (foundArgsInfo)
                return (null, null);

            var resultArgs = processedArgs.ToDictionary(
                                kvp => kvp.Key.Replace("--", string.Empty), // Transform the key
                                kvp => kvp.Value          // Keep the value as is
                            );

            return (resultArgs, result);
        }

        private static void DisplayArgumentsInfo(params CliArgument[] args)
        {
            int padding = 20;
            foreach (var a in args)
            {
                Console.WriteLine("Argument Information:");
                Console.WriteLine("\tName:".PadRight(padding) + a.Name.ToUpper());
                Console.WriteLine("\tDescription:".PadRight(padding) + a.Description);
                Console.WriteLine("\tOptional:".PadRight(padding) + a.IsOptional);
                Console.WriteLine("\tAccepted Values:".PadRight(padding) + "[" + string.Join("|", a.Values ?? new List<string>()) + "]");
                Console.WriteLine("\tStatic Value:".PadRight(padding) + (a.StaticValue == null || a.StaticValue == string.Empty ? "NULL" : a.StaticValue));
                Console.WriteLine("\tDefault Value:".PadRight(padding) + (a.Default == null || a.Default == string.Empty ? "NULL" : a.Default));
            }
        }

        private static bool ValidateNamesArguments(Dictionary<string, string>? namesArgsMap, string baseName)
        {
            List<string> unknownNames = new();

            if (namesArgsMap != null && namesArgsMap.Any())
            {

                var allSubCommandNames = CliHandler.GetSubCommandsBaseName(baseName, namesArgsMap.Keys.ToList()).Select(x => x.Name);

                foreach (var nameMap in namesArgsMap)
                {
                    if (!allSubCommandNames.Contains(nameMap.Key))
                    {
                        unknownNames.Add(nameMap.Key);
                    }
                }
            }
            if (unknownNames.Any())
            {
                string unknownArgsText = "Unknown Command Names:\n" + string.Join("\n", namesArgsMap.Keys.Select(x => x.ToUpper()));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(unknownArgsText);
                Console.ForegroundColor = ConsoleColor.White;

                return false;
            }
            return true;
        }

        private static bool ValidateVariableArguments(List<ICommand>? foundCommands, Dictionary<string, string>? variableArgsMap)
        {
            var allCommandsArgs = foundCommands?.SelectMany(x => x.Arguments ?? new()).Select(x => x.Name).ToList();

            if (allCommandsArgs == null)
            {
                return false;
            }
            List<string> unknownArgs = new();
            foreach (var item in variableArgsMap)
            {
                if (!allCommandsArgs.Contains(item.Key))
                {
                    unknownArgs.Add(item.Key);
                }
            }
            if (unknownArgs.Any())
            {
                string unknownArgsText = "Unknown Arguments:\n" + string.Join("\n", unknownArgs.Select(x => "--" + x.ToUpper()));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(unknownArgsText);
                Console.ForegroundColor = ConsoleColor.White;

                return false;
            }

            return true;
        }

    }
}
