using NodeSystem.Commands.Base;
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
            if (paramters is not Dictionary<string, string>)
            {
                return;
            }

            var args = (Dictionary<string, string>)paramters;

            if(args.ContainsKey("new"))
            {
                INode? n = CmdManager.CreateNodeByName(args["new"]);
                if (n != null && args.ContainsKey("n"))
                {
                    n.Name = args["n"].ToLower();
                }

                if(n != null)
                {

                    Console.Write($"new Node (");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{n.Name}");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write($") is created with ID (");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"${n.Guid}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($")");
                }
                else
                {
                    DisplayArgsInfo();
                }
            }
        }

    }
}
