using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands.Base
{
    public class CliCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICommand Cmd { get; set; }

        public List<ICommand> SubCommands { get; set; } = new(); // For nested commands
        public List<ICommand> EffectiveSubCommands => SubCommands.Count != 0 ? SubCommands : new List<ICommand>() { Cmd };

    }
}
