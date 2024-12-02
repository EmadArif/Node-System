using NodeSystem.Commands.Base;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands.Nodes
{
    internal class NodeInfoCommand : CommandBase
    {
        public override string Name => "info";

        public override string Description => "Get all node information";
        public override List<CliArgument> Arguments => new()
        {
            new CliArgument { Name = "i", Description = "Show node info by Id.", IsOptional = true },
        };
        public override void Execute(object paramters)
        {
            if (paramters is not Dictionary<string, string>)
            {
                return;
            }

            var args = (Dictionary<string, string>)paramters;

            if (args != null && args.ContainsKey("i"))
            {
                INode? node = CmdManager.CreatedNodes.FirstOrDefault(x => x.Guid.ToString().ToLower() == args["i"].ToLower());
               
                if (node != null)
                {
                    Console.WriteLine("");

                    Console.WriteLine("NAME:".PadRight(10) + node.Name.ToUpper().PadRight(30) + node.Description);
                    Console.WriteLine("");

                    if (node.InputTypes != null)
                    {
                        Console.Write("Input Types:".PadRight(20));

                        foreach (var input in node.InputTypes)
                        {
                            Console.Write($"{input.Name.ToUpper()}\t");
                        }
                    }
                    Console.WriteLine("");

                    if (node.OutputTypes != null)
                    {
                        Console.Write("Output Types:".PadRight(20));

                        foreach (var output in node.OutputTypes)
                        {
                            Console.Write($"{output.Name.ToUpper()}\t");
                        }
                    }
                    Console.WriteLine("\n");

                    if (node.AllConnectedNodes != null && node.AllConnectedNodes.Count > 0)
                    {
                        Console.WriteLine("Connected Nodes:".PadRight(20));

                        foreach (var output in node.AllConnectedNodes)
                        {
                            Console.WriteLine($"-{output.Name.ToUpper().PadRight(30) + output.Guid}\t");
                        }
                    }
                    Console.WriteLine("");
                }
            }

        }

        public override bool ValidateName(string cmdName)
        {
            return string.IsNullOrEmpty(cmdName);
        }
    }
}
