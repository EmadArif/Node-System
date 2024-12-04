using NodeSystem.CliSystem.Data;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NodeSystem.CliSystem.Extensions
{
    public static class CliExtension
    {
        public static List<Argument> ExtractAnyArguments(this string cmd, List<string> noneArguments)
        {
            string[] arrArgs = Regex.Replace(cmd.Trim(), @"\s+", ",").Split(',');

            List<Argument> argResult = new List<Argument>();
            int paramIndex = 0;

            while (paramIndex < arrArgs.Length)
            {
                string argeKey = arrArgs[paramIndex].Trim().ToLower();

                if (argeKey.Length == 0 || argResult.Select(x => x.Key).Contains(argeKey))
                {
                    paramIndex++;
                    continue;
                }

                if (paramIndex == 0)
                {
                    string? argValue = null;
                    if (paramIndex + 1 < arrArgs.Length)
                    {
                        argValue = arrArgs[paramIndex + 1].Trim().ToLower();

                        if (!(argValue.StartsWith("--") || argValue.Length == 0 || !noneArguments.Contains(argValue)))
                        {
                            paramIndex++;
                        }
                    }
                    else
                    {
                        argValue = null;
                    }
                    if (argValue != null)
                        argResult.Add(new Argument(ArgumentType.BASE, argeKey, argValue));
                }
                //arguments
                else if (argeKey.StartsWith("--"))
                {
                    string? argValue = null;
                    if (paramIndex + 1 < arrArgs.Length)
                    {
                        argValue = arrArgs[paramIndex + 1].Trim().ToLower();
                        if (argValue.StartsWith("--") || argValue.Length == 0)
                        {
                            argValue = null;
                        }
                        else
                        {
                            paramIndex++;
                        }
                    }
                    else
                    {
                        argValue = null;
                    }

                    argResult.Add(new Argument(ArgumentType.VARIABLE, argeKey, argValue));
                }
                else if (paramIndex + 1 < arrArgs.Length)
                {
                    string? argValue = arrArgs[paramIndex + 1].Trim().ToLower();
                    if(argValue != null)
                    {
                        paramIndex++;
                    }
                    /*if (!noneArguments.Contains(argValue))
                    {
                        argValue = null;
                    }
                    else
                    {
                        paramIndex++;
                    }*/
                    argResult.Add(new Argument(ArgumentType.NAME, argeKey, argValue));
                }

                paramIndex++;
            }

            return argResult;
        }
    }
}
