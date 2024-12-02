using NodeSystem.Commands.Base;
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
            if (paramters is not string)
            {
                return;
            }

            string? cmdText = ((string)paramters).ToLower();

            if (CmdManager.SelectedNode == null)
            {
                Console.WriteLine("There is no selected node.");
                return;
            }

            var optArgs = cmdText.ExtractArguments(Name, OptionalArgs);

            if(optArgs != null)
            {
                INode? childNode;

                if (optArgs.ContainsKey("new"))
                {
                    INode? node = CmdManager.CreateNodeByName(optArgs["to"]);
                    if(node != null)
                    {
                        if (optArgs.ContainsKey("n"))
                            node.Name = optArgs["n"];

                        childNode = node;

                        CmdManager.SelectedNode.AddConnection(childNode);
                        new NodeManager().DrawConnectedNodes(CmdManager.SelectedNode);
                    }
                    else
                    {
                        Console.WriteLine($"Node ({optArgs["new"]}) is not exists.");
                    }
                }
                else if (optArgs.ContainsKey("to"))
                {
                    string nodeNameOrId = optArgs["to"];

                    childNode = CmdManager.CreatedNodes.FirstOrDefault(x => x.Guid.ToString().ToLower() == nodeNameOrId.ToLower() || x.Name.ToLower() == nodeNameOrId.ToLower());
                    if (childNode == null)
                    {
                        Console.WriteLine($"Node ({nodeNameOrId}) is not exists.");
                        return;
                    }

                    CmdManager.SelectedNode.AddConnection(childNode);
                    new NodeManager().DrawConnectedNodes(CmdManager.SelectedNode);
                }
                else if (optArgs.ContainsKey("d"))
                {
                    string id = optArgs["d"];
                    bool deleted = CmdManager.DeleteConnectionFromSelected(id);
                    if(deleted)
                    {
                        Console.WriteLine($"Node with Id:({id}) is deleted from connection.");
                        new NodeManager().DrawConnectedNodes(CmdManager.SelectedNode);

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
