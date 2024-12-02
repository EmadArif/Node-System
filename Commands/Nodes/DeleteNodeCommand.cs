using NodeSystem.Commands.Base;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands.Nodes
{
    internal class DeleteNodeCommand : CommandBase
    {
        public override string Name => "delete";

        public override string Description => "Delete exiting node or all nodes";
        public override List<CliArgument> Arguments => new()
        {
            new CliArgument { Name = "d", Description = "Delete existing node by Id." },
        };
        public override void Execute(object paramters)
        {
            if (paramters is not Dictionary<string, string>)
            {
                return;
            }

            var args = (Dictionary<string, string>)paramters;

            if (args != null && args.ContainsKey("d"))
            {
                string id = args["d"];
                if (CmdManager.DeleteCreatedNodeById(id))
                {
                    Console.WriteLine($"Node with Id ({id}) has been deleted.");
                }
            }
        }
    }
}
