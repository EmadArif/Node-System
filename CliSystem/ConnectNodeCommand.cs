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
    internal class ConnectNodeCommand : CommandBase
    {
        public override string Name => "connect";
        public override string Description => "Connect selected node with a node.";
        public override List<CliArgument> Arguments => new()
        {
            new CliArgument { Name = "to", Description = "Connect selected node to a node, VALUES: <NAME> or <ID>", IsOptional = true },
            new CliArgument { Name = "d", Description = "Delete connected node by Id.", IsOptional = true },
            new CliArgument { Name = "new", Description = "Connect to a new created node.", IsOptional = true, StaticValue = "Y" },
            new CliArgument { Name = "n", Description = "Set nick name to the node.", IsOptional = true },
        };

        public override void Execute(object paramters)
        {
            if (paramters is not Dictionary<string, string> || paramters == null)
            {
                return;
            }

            if (CliManager.Shared.SelectedNode == null)
            {
                Console.WriteLine("There is no selected node.");
                return;
            }

            var args = (Dictionary<string, string>)paramters;

            if (args != null)
            {
                INode? childNode;

                if (args.ContainsKey("new"))
                {
                    INode? node = CliManager.CreateNodeByName(args["to"]);
                    if(node != null)
                    {
                        if (args.ContainsKey("n"))
                            node.Name = args["n"];

                        childNode = node;
                        if(CliManager.Shared.SelectedNode != null)
                        {
                            CliManager.Shared.SelectedNode?.AddConnection(childNode);
                            new NodeManager().DrawConnectedNodes(CliManager.Shared.SelectedNode!);
                        }
                     
                    }
                    else
                    {
                        Console.WriteLine($"Node ({args["new"]}) is not exists.");
                    }
                }
                else if (args.ContainsKey("to"))
                {
                    string nodeNameOrId = args["to"];

                    childNode = CliManager.Shared.CreatedNodes.FirstOrDefault(x => x.Guid.ToString().ToLower() == nodeNameOrId.ToLower() || x.Name.ToLower() == nodeNameOrId.ToLower());
                    if (childNode == null)
                    {
                        Console.WriteLine($"Node ({nodeNameOrId}) is not exists.");
                        return;
                    }

                    CliManager.Shared.SelectedNode.AddConnection(childNode);
                    new NodeManager().DrawConnectedNodes(CliManager.Shared.SelectedNode);
                }
                else if (args.ContainsKey("d"))
                {
                    string id = args["d"];
                    bool deleted = CliManager.DeleteConnectionFromSelected(id);
                    if(deleted)
                    {
                        Console.WriteLine($"Node with Id:({id}) is deleted from connection.");
                        new NodeManager().DrawConnectedNodes(CliManager.Shared.SelectedNode);

                    }
                    else
                    {
                        Console.WriteLine($"Node with Id:({id}) is not exists.");
                    }
                }
            }
           
        }
    }
}
