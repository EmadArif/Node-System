using NodeSystem.CliSystem.Core;
using NodeSystem.Commands.Base;
using NodeSystem.Commands.Data;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands
{
    public class NodesListCommand : CommandBase
    {
        public override string Name => "nodes";
        public override string Description => "Show all created nodes";
        public override List<CliArgument> Arguments => new()
        {
            new CliArgument { Name = "ls",Default = "all", Description = "List all created node, VALUES: all | <NAME> | selected", IsOptional = true },
        };

        public override void Execute(object paramters)
        {
            if(paramters is not Dictionary<string, string>)
            {
                return;
            }

            var args = (Dictionary<string, string>)paramters;

            if (args != null && args.ContainsKey("ls"))
            {
                List<INode> nodes = new List<INode>();

                string value = args["ls"];
                if (value.ToLower() == "all")
                {
                    nodes = CliManager.Shared.CreatedNodes;
                }
                else if (value.ToLower() == "selected")
                {
                    if (CliManager.Shared.SelectedNode == null)
                    {
                        Console.WriteLine("There is no selected node.");
                        return;
                    }
                    else
                    {
                        nodes.Add(CliManager.Shared.SelectedNode);
                    }
                }
                else
                {
                    nodes = CliManager.Shared.CreatedNodes.Where(x => x.Name.ToLower() == value.ToLower()).ToList();
                }

                DisplayNodes(nodes);
            }

            return;
            if (paramters is not string)
            {
                return;
            }

            string? cmdText = ((string)paramters).ToLower();

            var optArgs = cmdText.ExtractArguments(Name, OptionalArgs);
            if (optArgs != null && optArgs.ContainsKey("d"))
            {
                string id = optArgs["d"];
                if (CliManager.DeleteCreatedNodeById(id))
                {
                    Console.WriteLine($"Node with Id (${id}) has been deleted.");
                }
            }
            if (optArgs != null && optArgs.ContainsKey("new"))
            {
                INode? n = CliManager.CreateNodeByName(optArgs["new"]);
                if(n != null && optArgs.ContainsKey("n"))
                {
                    n.Name = optArgs["n"].ToLower();
                }
                DisplayNodes(CliManager.Shared.CreatedNodes);

            }
            if (optArgs != null && optArgs.ContainsKey("ls"))
            {
                List<INode> nodes = new List<INode>();

                string value = optArgs["ls"];
                if (value.ToLower() == "all")
                {
                    nodes = CliManager.Shared.CreatedNodes;
                }
                else if (value.ToLower() == "selected")
                {
                    if (CliManager.Shared.SelectedNode == null)
                    {
                        Console.WriteLine("There is no selected node.");
                        return;
                    }
                    else
                    {
                        nodes.Add(CliManager.Shared.SelectedNode);
                    }
                }
                else
                {
                    nodes = CliManager.Shared.CreatedNodes.Where(x => x.Name.ToLower() == value.ToLower()).ToList();
                }

                DisplayNodes(nodes);
            }
            else if(optArgs == null || optArgs.Count == 0)
            {
                Console.WriteLine("-----------------");
                foreach (var n in CliManager.Shared.Nodes)
                {
                    Console.WriteLine(n.Name.ToUpper().PadRight(40) + n.Description);
                }
                Console.WriteLine("-----------------");
            }
            if (optArgs != null && optArgs.ContainsKey("i"))
            {
                INode? node = CliManager.Shared.CreatedNodes.FirstOrDefault(x => x.Guid.ToString().ToLower() == optArgs["i"].ToLower());
                if(node != null)
                {
                    ShowNodeInfo(node);
                }
            }
        }

        private static void DisplayNodes(List<INode> nodes)
        {
            Console.WriteLine("-----------------");
            foreach (var n in nodes)
            {
                if (CliManager.Shared.SelectedNode != null && n.Guid == CliManager.Shared.SelectedNode.Guid)
                {
                    CliConsole.WriteLineSuccess(n.Name.PadRight(30) + n.Guid);
                }
                else
                {
                    Console.WriteLine(n.Name.PadRight(30) + n.Guid);
                }
            }
            Console.WriteLine("-----------------");
        }

        private void ShowNodeInfo(INode node)
        {
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
}
