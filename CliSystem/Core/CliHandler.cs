using NodeSystem.CliSystem.Core;
using NodeSystem.CliSystem.Data;
using NodeSystem.Commands.Data;
using NodeSystem.Commands.Nodes;
using System.Text.RegularExpressions;

namespace NodeSystem.Commands.Base
{
    public static class CliHandler
    {

        public static readonly List<CliCommand> Commands = new()
        {

            //Nodes List Command
            new CliCommand
            {
                Name = "nodes",
                SubCommands = new()
                {
                    new DeleteNodeCommand(),
                    new CreateNodeCommand(),
                    new NodeInfoCommand(),
                    new NodesListCommand(),
                }
            },
            new CliCommand
            {
                Name = "set",
                //Cmd = new NodesListCommand(),
                SubCommands = new()
                {
                    new SetNodeCommand()
                }
            },
            new CliCommand
            {
                Name = "draw",
                Cmd = new DrawNodeTreeCommand(),
            },
            new CliCommand
            {
                Name = "connect",
                Cmd = new ConnectNodeCommand(),
            },
            //Nodes List Command
            new CliCommand
            {
                Name = "help",
                Cmd = new HelpCommand(),
                SubCommands = new()
                {
                    new HelpCommand(),
                }
            },
             //Nodes List Command
            new CliCommand
            {
                Name = "cls",
                Cmd = new ClearScreenCommand(),
            },
        };
        private static List<ICommand> _allCommands = new List<ICommand>();

        public static List<ICommand> AllCommands
        { 
            get
            {
                if(_allCommands.Count == 0)
                {
                    _allCommands = Commands.SelectMany(x => x.EffectiveSubCommands).ToList();
                }
                return _allCommands;
            }
        }

        public static List<ICommand> GetCommandsByBaseName(string baseName)
        {
            var baseCommand = Commands.FirstOrDefault(cmd => baseName.Equals(cmd.Name, StringComparison.OrdinalIgnoreCase));
            if (baseCommand == null)
            {
                return new List<ICommand>(); // Return an empty list if the base command is not found
            }
            List<ICommand> allCommands = new List<ICommand>();
            if(baseCommand.Cmd != null)
            {
                allCommands.Add(baseCommand.Cmd);
            }
            allCommands.AddRange(baseCommand.SubCommands);

            return allCommands;
        }

        public static List<ICommand> GetSubCommandsBaseName(string baseName, List<string> subNames)
        {
            var subCommand = GetCommandsByBaseName(baseName).Where(x => subNames.Contains(x.Name));
            if (subCommand == null)
                return new List<ICommand>();
            return subCommand.ToList();
        }

        public static List<ICommand> GetCommandsByName(string baseName, List<string> subNames)
        {
            // Find the base command
            var baseCommand = Commands.FirstOrDefault(cmd => baseName.Equals(cmd.Name, StringComparison.OrdinalIgnoreCase));
            if (baseCommand == null)
            {
                return new List<ICommand>(); // Return an empty list if the base command is not found
            }

            // If no subNames are provided, return all subcommands
            if (subNames == null || subNames.Count == 0 || subNames.Contains("all"))
            {
                return baseCommand.SubCommands;
            }

            // Filter subcommands that match the provided subNames (case-insensitive)
            var matchingSubCommands = baseCommand.SubCommands
                .Where(subCmd => subNames.Any(name => name.Equals(subCmd.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return matchingSubCommands;
        }


        public static List<ICommand> GetCommands(List<string> paramters, List<CliCommand> clis)
        {
            List<ICommand> cmds = new();
            string baseName = paramters[0];
            foreach (var item in clis)
            {
                if(baseName.Equals(item.Name))
                {
                    List<ICommand> foundCommands = new List<ICommand>();
                    paramters = paramters.Skip(1).ToList();
                    if (paramters.Count == 1 && paramters[0] == "?")
                    {
                        foundCommands.AddRange(item.SubCommands);
                        break;
                    }
                    if (item.SubCommands != null)
                    {
                        foreach (var subCmd in item.SubCommands)
                        {
                            bool? found = CheckArguments(subCmd, paramters);
                            if(found == null)
                            {
                                continue;
                            }
                            if (!found.Value)
                            {
                                var message = subCmd.ValidateArgs(paramters.ToArray());

                                if(message != null)
                                {
                                    CliConsole.WriteLineError(message);

                                }
                                break;
                            }
                            else
                            {
                                foundCommands.Add(subCmd);
                            }
                            
                        }
                    }

                    if(foundCommands.Count > 0)
                    {
                        cmds.AddRange(foundCommands);
                    }else if(item.Cmd != null)
                    {
                        cmds.Add(item.Cmd);
                    }
                    break;
                }
            }
         /*   if (paramters.Count > 0 && cmds.Count == 0)
            {
                string g = string.Join("->", paramters);
                Console.WriteLine($"[{g}] is not recognized as command.");
            }*/

            return cmds;
        }

        private static bool? CheckArguments(ICommand subCmd, List<string> paramters)
        {
            var foundArguments = subCmd.Arguments.Where(x => paramters.Contains(x.Name.ToLower())).ToList();

            if (foundArguments == null || foundArguments.Count == 0)
                return null;

            var requiredArguments = subCmd.Arguments.Where(x => !x.IsOptional).ToList();
            if (requiredArguments.Any())
            {
                bool found = CheckAllArgumentsExists(foundArguments, requiredArguments);
                return found;
            }

            return true;
        }

        public static bool CheckAllArgumentsExists(List<CliArgument> source, List<CliArgument> target)
        {
            // Use HashSet for efficient lookups with case-insensitivity
            var sourceNames = new HashSet<string>(source.Select(x => x.Name), StringComparer.OrdinalIgnoreCase);

            // Check if all target argument names exist in the source
            return target.All(targArg => sourceNames.Contains(targArg.Name));
        }


    }
}
