using NodeSystem.Commands.Base;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands
{
    internal class DisplayNodeTreeCommand : CommandBase
    {
        public override string Name => "DRAW";

        public override string Description => "Draw the full path of the selected node.";
        public override List<CliArgument> Arguments => new()
        {
            new CliArgument { Name = "node", Description = "Display node connection tree", IsOptional = true },
            new CliArgument { Name = "a", Description = "Display full tree with id", IsOptional = true, StaticValue = "all" },
        };

        public override void Execute(object paramters)
        {
            if (paramters is not string)
            {
                return;
            }

            string? cmdText = ((string)paramters).ToLower();
            var optArgs = cmdText.ExtractArguments(Name, OptionalArgs);

            if (optArgs != null && optArgs.ContainsKey("a"))
            {
                if(CmdManager.SelectedNode != null)
                {
                    DrawNodeTree(CmdManager.SelectedNode, true);
                }
                else
                {
                    Console.WriteLine($"No Selected node.");
                }
            }
            else if (optArgs != null && optArgs.ContainsKey("node"))
            {
                var name = optArgs["node"];

                var nodes = CmdManager.GetAllNodes();
                INode? selectedNode = nodes.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
                if (selectedNode == null)
                {
                    Console.WriteLine($"Please provide node name");
                    return;
                }
                DrawNodeTree(selectedNode);

            }
            else
            {
                if (CmdManager.SelectedNode == null)
                {
                    Console.WriteLine($"No Selected node.");
                    return;
                }
                DrawNodeTree(CmdManager.SelectedNode);

            }



        }
        void DrawNodeTree(INode? node, bool details = false)
        {
            if (node != null)
                new NodeManager().DrawConnectedNodes(node, details);
            else
                Console.WriteLine("There is node selected node!");

            return;
        }
    }
}
