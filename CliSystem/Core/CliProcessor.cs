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

        private static bool ValidateChar(char c)
        {
            if (char.IsDigit(c) || char.IsLetter(c) || char.IsNumber(c) || c == '-' || noneArguments.Contains(c.ToString()) || c == '\r' || c == '\b' || c == ' ')
            {
                return true;
            }

            return false;
        }
        public static string ProcessInputs(List<CliCommand> commands)
        {
            HashSet<CliKeyword> keywords = GetKeywords(commands);
            string typedText = string.Empty;
            const string prompt = "CMD>";
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(prompt); // Display the initial prompt
            
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true); // Read key without displaying it

                if (keyInfo.Key == ConsoleKey.Tab)
                {
                    foreach (var item in keywords)
                    {
                        var words = typedText.Split(' ');
                        if(words.Length > 0)
                        {
                            string lastWord = words[words.Length - 1].Trim();
                            if (item.Name != lastWord && lastWord != string.Empty && item.Name.Contains(lastWord))
                            {
                                words[words.Length - 1] = item.Name;
                                typedText = string.Join(' ', words);
                                typedText += " ";
                                ResetCmdText(prompt);
                                HighlightKeywords(typedText, keywords);

                                break;
                            }
                        }
                        
                    }
                }

                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (savedInputIndex - 1 >= 0)
                    {
                        typedText = savedInputs[savedInputIndex-1];
                        savedInputIndex--;
                    }
                    else
                    {
                        typedText = string.Empty;
                        savedInputIndex = -1;
                    }

                    ResetCmdText(prompt);
                    HighlightKeywords(typedText, keywords);
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (savedInputIndex + 1 < savedInputs.Count)
                    {
                        typedText = savedInputs[savedInputIndex+1];
                        savedInputIndex++;
                        ResetCmdText(prompt);

                        HighlightKeywords(typedText, keywords);
                    }
                }

                if (!ValidateChar(keyInfo.KeyChar))
                    continue;
                
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    savedInputs.Add(typedText);
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (typedText.Length > 0)
                    {
                        // Handle Backspace: remove the last character
                        typedText = typedText.Substring(0, typedText.Length - 1);

                        // Remove last character from the console
                        Console.Write("\b \b");

                        ResetCmdText(prompt);
                        HighlightKeywords(typedText, keywords);
                    }
                }
                else
                {

                    // Append the key character to the typed text
                    typedText += keyInfo.KeyChar;
                    typedText = typedText.ToLower();

                    ResetCmdText(prompt);

                    HighlightKeywords(typedText, keywords);

                    // Reset color at the end
                    Console.ResetColor();
                }
            }

            return typedText;
        }

        private static void ResetCmdText(string baseText)
        {
            Console.Write("\r" + new string(' ', Console.BufferWidth - 1)); // Clear current line
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Write("\r" + baseText); // Write the prompt
        }

        private static HashSet<CliKeyword> GetKeywords(List<CliCommand> commands)
        {
            HashSet<CliKeyword> keywords = new();
            foreach (var item in commands)
            {
                keywords.Add(new CliKeyword { Name = item.Name.ToLower(), type = CliKeywordType.BASE });
                if(item.Cmd != null)
                {
                    var arguments = item.Cmd.Arguments?.Select(arg => arg.Name).ToList();
                    if (arguments != null)
                    {
                        arguments.ForEach(x =>
                        {
                            keywords.Add(new CliKeyword { Name = $"--{x}", type = CliKeywordType.VARIABLE });
                        });

                    }
                }
                if(item.SubCommands != null)
                {
                    var arguments = item.SubCommands.SelectMany(x => x.Arguments ?? new List<CliArgument>()).Select(arg => arg.Name).ToList();
                    if (arguments != null)
                    {
                        arguments.ForEach(x =>
                        {
                            keywords.Add(new CliKeyword { Name = $"--{x}", type = CliKeywordType.VARIABLE });
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

        private static void HighlightKeywords(string input, HashSet<CliKeyword> keywords)
        {
            // Split the input into words and track positions
            var words = input.Split(new[] { ' ' }, StringSplitOptions.None);
            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i];
                var foundKeyword = keywords.FirstOrDefault(x => x.Name.ToLower().Equals(word));
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

            if (baseArgsMap.Any() && noneArguments.Contains(baseArgsMap.First().Value))
            {
                var cmdByNames = CliHandler.GetCommandsByBaseName(baseArgsMap.Keys.First());
                foreach (var cmd in cmdByNames)
                {
                    cmd.DisplayArgsInfo();
                }
                if (!cmdByNames.Any())
                {
                    Console.WriteLine($"{cmdText} is not recognized as command.");
                }

                return (null, null);
            }
            else if (namesArgsMap.Count > 0)
            {
                var cmdByNames = CliHandler.GetSubCommandsBaseName(baseName, namesArgsMap.Keys.ToList());

                foreach (var cmd in cmdByNames)
                {
                    cmd.DisplayArgsInfo();
                }
            }

            var resultArgs = processedArgs.ToDictionary(
                                kvp => kvp.Key.Replace("--", string.Empty), // Transform the key
                                kvp => kvp.Value          // Keep the value as is
                            );

            return (resultArgs, result);
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
