using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.CliSystem.Data
{
    public enum ArgumentType
    {
        BASE,
        VARIABLE,
        NAME,
    }

    public struct Argument
    {
        public ArgumentType ArgType;
        public string Key;
        public string? Value;

        public Argument(ArgumentType argType, string key, string? value)
        {
            ArgType = argType;
            Key = key;
            Value = value;
        }
    }
}
