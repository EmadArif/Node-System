using NodeSystem.Commands.Base;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands
{
    public class SetNodeCommand : CommandBase, INodeCommand
    {
        public override string Name => "select";

        public override string Description => "Set selected node by it's number.";
        public override List<CliArgument>? Arguments => new()
        {
            new CliArgument { Name = "cs", Description = "Create and select a new node.", IsOptional = true, Values = new() { "<NAME>" } },
            new CliArgument { Name = "s", Description = "Select an existing node by Id.", IsOptional = true, Values = new() { "<GUID>" } },
            new CliArgument { Name = "n", Description = "Set nick name to the node.", IsOptional = true, Values = new() { "<NICK-NAME>" } },
        };
        public override void Execute(object paramters)
        {

            if (paramters is not Dictionary<string, string> || paramters == null)
            {
                return;
            }

            var args = (Dictionary<string, string>)paramters;
            INode? selectedNode = null;
            string name = string.Empty;

            if (args.ContainsKey("cs"))
            {
                selectedNode = CmdManager.CreateNodeByName(args["cs"]);
                name = args["cs"];

                if (selectedNode != null)
                {
                    if (args.ContainsKey("n"))
                    {
                        selectedNode.Name = args["n"];
                        name = selectedNode.Name;
                    }

                    CmdManager.SetSelectedNode(selectedNode);
                }
            }
            else if (args.ContainsKey("id"))
            {
                selectedNode = CmdManager.CreatedNodes.FirstOrDefault(x => x.Guid.ToString() == args["id"]);
                name = args["id"];
            }

            if (selectedNode == null)
            {
                if (name.Length > 0)
                    Console.WriteLine($"Node ({name}) is not exists.");
            }
            else
            {
                DisplaySelectedNode();
            }

            return;

            if (paramters is not string)
            {
                return;
            }
            
            string? cmdText = ((string)paramters).ToLower();

            var optArgs = cmdText.ExtractArguments(Name, OptionalArgs);
            

            if (optArgs != null && optArgs.ContainsKey("id"))
            {
                selectedNode = CmdManager.CreatedNodes.FirstOrDefault(x => x.Guid.ToString() == optArgs["id"]);
            }
            else if (optArgs != null && optArgs.ContainsKey("name"))
            {
                var nodes = CmdManager.GetAllNodes();
                selectedNode = nodes.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));

                if (optArgs.ContainsKey("n"))
                {
                    name = optArgs["n"];
                    selectedNode.Name = name;
                }
            }
            if (selectedNode == null)
            {
                if(name.Length > 0)
                    Console.WriteLine($"Node ({name}) is not exists.");
            }
            else
            {
                CmdManager.SetSelectedNode(selectedNode);
                DisplaySelectedNode();
            }

            if (optArgs != null && optArgs.ContainsKey("?"))
            {
                DisplayArgsInfo();
            }


        }

        private void DisplaySelectedNode()
        {

            Console.Write($"Node (");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{CmdManager.SelectedNode?.Name}");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write($") is selected with ID (");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"${CmdManager.SelectedNode?.Guid}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($")");
        }


        public override bool ValidateName(string cmdName)
        {
            return cmdName.StartsWith("set-node");
        }
    }
}
