using NodeSystem.Commands.Base;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands
{
    internal class ClearScreenCommand : CommandBase
    {
        public override string Name => "cls";

        public override string Description => "Create a new node";

        public override void Execute(object paramters)
        {
            if (paramters is not Dictionary<string, string>)
            {
                return;
            }

            Console.Clear();
        }
    }
}
