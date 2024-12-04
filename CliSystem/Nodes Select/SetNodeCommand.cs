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
    public class SetNodeCommand : CommandBase, INodeCommand
    {
        public override string Name => "set";

        public override string Description => "Set selected node by it's number.";
        public override List<CliArgument>? Arguments => new()
        {
            new CliArgument { Name = "cs", Description = "Create and select a new node.", IsOptional = true, Values = new() { "<NAME>" } },
            new CliArgument { Name = "s", Description = "Set Selected node by Id.", IsOptional = true, Values = new() { "<GUID>" } },
            new CliArgument { Name = "n", Description = "Rename or Set nick name to the node selected or newly created node.", IsOptional = true, Values = new() { "<NICK-NAME>" } },
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
                selectedNode = CliManager.CreateNodeByName(args["cs"]);
                name = args["cs"];

               
            }
            else if (args.ContainsKey("s"))
            {
                selectedNode = CliManager.Shared.CreatedNodes.FirstOrDefault(x => x.Guid.ToString() == args["s"]);
                name = args["s"];
            }

            if (selectedNode != null)
            {
                if (args.ContainsKey("n"))
                {
                    selectedNode.Name = args["n"];
                    name = selectedNode.Name;
                }

                CliManager.SetSelectedNode(selectedNode);
                DisplaySelectedNode();

            }
            else if (selectedNode == null)
            {
                if (name.Length > 0)
                    Console.WriteLine($"Node ({name}) is not exists.");
            }
            

            return;
        }

        private void DisplaySelectedNode()
        {

            Console.Write($"Node (");
            CliConsole.WriteSuccess($"{CliManager.Shared.SelectedNode?.Name}");
            Console.Write($") is selected with ID (");
            CliConsole.WriteSuccess($"{CliManager.Shared.SelectedNode?.Guid}");
            Console.WriteLine($")");
        }
    }
}
