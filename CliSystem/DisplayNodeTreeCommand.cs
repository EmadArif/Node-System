using NodeSystem.CliSystem.Core;
using NodeSystem.Commands.Base;
using NodeSystem.Commands.Data;
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
        public override string Name => "draw";

        public override string Description => "Draw the full path of the selected node.";
        public override List<CliArgument> Arguments => new()
        {
            new CliArgument { Name = "node", Description = "Display node connection tree", IsOptional = true },
            new CliArgument { Name = "a", Description = "Display full tree with id", IsOptional = true, StaticValue = "all" },
        };

        public override void Execute(object paramters)
        {
            if (paramters is not Dictionary<string, string> || paramters == null)
            {
                return;
            }

            var args = (Dictionary<string, string>)paramters;

            if (args != null && args.ContainsKey("a"))
            {
                if(CliManager.Shared.SelectedNode != null)
                {
                    DrawNodeTree(CliManager.Shared.SelectedNode, true);
                }
                else
                {
                    Console.WriteLine($"No Selected node.");
                }
            }
            else if (args != null && args.ContainsKey("node"))
            {
                var name = args["node"];

                var nodes = CliManager.Shared.CreatedNodes;
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
                if (CliManager.Shared.SelectedNode == null)
                {
                    Console.WriteLine($"No Selected node.");
                    return;
                }
                DrawNodeTree(CliManager.Shared.SelectedNode);

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
