using NodeSystem.CliSystem.Core;
using NodeSystem.Commands.Base;
using NodeSystem.Commands.Data;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands.Nodes
{
    public class CreateNodeCommand : CommandBase
    {
        public override string Name => "create";

        public override string Description => "Create a new node";
        public override List<CliArgument> Arguments => new()
        {
            new CliArgument { Name = "new", Description = "Create a new node", Values = new () { "<NAME>" } },
            new CliArgument { Name = "n", Description = "Set nick name to the new node.", IsOptional = true, Values = new (){ "<NICK-NAME>" } },

        };
        public override void Execute(object paramters)
        {
            if (paramters is not Dictionary<string, string> || paramters == null)
            {
                return;
            }

            var args = (Dictionary<string, string>)paramters;

            if(args.ContainsKey("new"))
            {
                INode? n = CliManager.CreateNodeByName(args["new"]);
                if (n != null && args.ContainsKey("n"))
                {
                    n.Name = args["n"].ToLower();
                }

                if(n != null)
                {
                    Console.Write($"new Node (");
                    CliConsole.WriteSuccess(n.Name);
                    Console.Write($") is created with ID (");
                    CliConsole.WriteSuccess(n.Guid);
                    Console.WriteLine($")");
                }
                else
                {
                    CliConsole.WriteLineWarning($"Node with name {args["new"]} not found!");

                }
            }
        }

    }
}
