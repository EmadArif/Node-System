using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands.Base
{
    public class CliArgument
    {
        public string Name { get; set; }
        public string Description = string.Empty;
        public List<string> Values { get; set; }
        public string? Default = null;
        public string? StaticValue = null;
        public bool IsOptional = false;
    }
}
