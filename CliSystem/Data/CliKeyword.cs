using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.CliSystem.Data
{
    public enum CliKeywordType
    {
        BASE,
        VARIABLE,
        NONE,
    }
    public class CliKeyword
    {
        public string CommandName { get; set; }
        public string Name { get; set; }
        public CliKeywordType type { get; set; }
    }

    public class CliKeywordList
    {
        public string BaseCommandName = string.Empty;
        public List<CliKeyword>? Keywords;
    }

}
