using NodeSystem.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands
{
    public interface ICommand
    {
        public Guid Id { get; }
        public string Name { get; }
        public List<CliArgument>? Arguments { get; }
        public string Description { get; }
        void Execute(object paramters);
        void DisplayArgsInfo();
        string? ValidateArgs(string[] insertedArgs);
        void ValidateArgs(Dictionary<string, string> insertedArgs);
    }
}
