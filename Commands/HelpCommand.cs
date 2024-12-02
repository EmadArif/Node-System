using NodeSystem.Commands.Base;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands
{
    public class HelpCommand : CommandBase
    {
        public override string Name => "HELP";

        public override string Description => "Show all available commands";
        public override List<CliArgument> Arguments => new()
        {
            new CliArgument { Name = "a", Description = "Show all available commands.", IsOptional = true, Default = "all", Values = new (){ "all", "less", "<NAME>" } },
        };

        public override void Execute(object paramters)
        {
            if (paramters is not Dictionary<string, string>)
            {
                return;
            }

            var args = (Dictionary<string, string>)paramters;

            if(args != null && args.Count > 0)
            {
                if (args.ContainsKey("a"))
                {
                    if(args["a"] == "all")
                    {
                        foreach (var cmd in CliManager.AllCommands)
                        {
                            cmd.DisplayArgsInfo();
                        }
                    }
                    else if(args["a"] == "less")
                    {
                        foreach (var cmd in CliManager.AllCommands)
                        {
                            Console.WriteLine("Command: " + cmd.Name.ToUpper().PadRight(42) + $" ({cmd.Description})");
                        }
                    }
                    else
                    {
                        foreach (var cmd in CliManager.AllCommands.Where(x => x.Name.ToLower().Equals(args["a"])))
                        {
                            cmd.DisplayArgsInfo();
                        }
                    }
                }
            }
        }
        public override bool ValidateName(string name)
        {
            return name == "help" || name.StartsWith("help ");
        }
    }
}
