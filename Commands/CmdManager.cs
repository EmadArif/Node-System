using NodeSystem.Commands.Base;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands
{
    public static class CmdManager
    {
        public static INode? SelectedNode { get; private set; }

        public static readonly List<ICommand> Commands = new ();
        public static readonly List<INode> CreatedNodes = new ();
        public static readonly Dictionary<string[], ICommand> CommandsMap = new()
        {
            { new string[] { "nodes" }, new NodesListCommand() },
            { new string[] { "nodes", "new", "n" }, new NodesListCommand() },
            { new string[] { "nodes", "?", "d" }, new NodesListCommand() },
        };


        public static void Init()
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Get all types in the assembly
            var allTypes = assembly.GetTypes();

            // Filter for classes that can be instantiated (non-abstract and public)
            var nodeTypes = allTypes.Where(t => typeof(ICommand).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            foreach (var type in nodeTypes)
            {
                var instance = (ICommand)Activator.CreateInstance(type)!;
                Commands.Add(instance);
            }
        }

        public static Dictionary<string[], ICommand>? GetLongestCommands(List<string> commandParts)
        {
            Dictionary<string[], ICommand> keys = new();
            foreach (var item in CommandsMap)
            {
                bool found = true;
                foreach (var k1 in item.Key)
                {
                    if (!commandParts.Contains(k1))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    keys.Add(item.Key, item.Value);
                }
            }

            if(commandParts.Count > 0)
            {
                keys.Remove(new string[] { commandParts[0] });
            }


            return keys;
        }


        public static (Dictionary<string, string>, List<ICommand>) ExtractCommands(string cmdText)
        {
            List<ICommand> result = new ();
            string[] names = cmdText.Split(' ');

            string baseName = names.First();
            List<string> noneArguments = new () { "?" };

            var allArgs = cmdText.ExtractAnyArguments(noneArguments, baseName);
                
            var variableArgsMap = allArgs.Where(x => x.ArgType == ArgumentType.VARIABLE).ToDictionary(
                                kvp => kvp.Key.Replace("--", string.Empty), // Transform the key
                                kvp => kvp.Value          // Keep the value as is
                            );
            var baseArgsMap = allArgs.Where(x => x.ArgType == ArgumentType.BASE).ToDictionary(
                                kvp => kvp.Key.Replace("--", string.Empty), // Transform the key
                                kvp => kvp.Value          // Keep the value as is
                            );
            var noneArgsMap = allArgs.Where(x => x.ArgType == ArgumentType.NONE).ToDictionary(
                               kvp => kvp.Key.Replace("--", string.Empty), // Transform the key
                               kvp => kvp.Value          // Keep the value as is
                           );

            List<string> inputs = new()
            {
                baseName
            };

            if (allArgs != null && allArgs.Count > 0)
            {
                inputs.AddRange(variableArgsMap.Keys);
                for (int i = 0; i < inputs.Count; i++)
                {
                    inputs[i] = inputs[i].Replace("--", string.Empty);
                }
            }

            var foundCommands = CliManager.GetCommands(inputs, CliManager.Commands);
            if(foundCommands != null)
            {
                foreach (var item in foundCommands)
                {
                    result.Add(item);
                }
            }

            Dictionary<string, string> processedArgs = new();
           
            foreach (var arg in variableArgsMap)
            {
                foreach (var item in result)
                {
                    string argKey = arg.Key.Replace("--", "");
                    var foundArg = item.Arguments?.SingleOrDefault(x => x.Name.ToLower() == argKey);
                    
                    if (foundArg != null)
                    {
                        string? value = foundArg!.StaticValue ?? arg.Value ?? foundArg!.Default;

                        if (value == null)
                        {
                            item.ValidateArgs(variableArgsMap);
                            return (processedArgs, new List<ICommand>());
                        }

                        processedArgs.Add(arg.Key, value!);

                        break;
                    }
                }
            }

            if (baseArgsMap.Any())
            {
                var cmdByNames = CliManager.GetCommandsByBaseName(baseArgsMap.Keys.First());
                foreach (var cmd in cmdByNames)
                {
                    cmd.DisplayArgsInfo();
                }
            }
            else if(noneArgsMap.Count > 0)
            {
                var cmdByNames = CliManager.GetSubCommandsBaseName(baseName, noneArgsMap.Keys.ToList());

                foreach (var cmd in cmdByNames)
                {
                    cmd.DisplayArgsInfo();
                }
            }
        

            var resultArgs = processedArgs.ToDictionary(
                                kvp => kvp.Key.Replace("--", string.Empty), // Transform the key
                                kvp => kvp.Value          // Keep the value as is
                            );

            return (resultArgs, result);
        }



        public static void SetSelectedNode(INode node)
        {
            SelectedNode = node;
            SetCreatedNode(node);
        }

        public static bool DeleteCreatedNodeById(string id)
        {
            INode? node = CreatedNodes.SingleOrDefault(t => t.Guid.ToString().ToLower() == id);
            if (node == null)
                return false;

            CreatedNodes.ForEach(x => x.DeleteConnection(node));
            return CreatedNodes.RemoveAll(x => x.Guid.ToString().ToLower() == id.ToLower()) > 0;
        }

        public static bool DeleteConnectionFromSelected(string id)
        {
            if (SelectedNode == null)
                return false;

            INode? node = CreatedNodes.SingleOrDefault(t => t.Guid.ToString().ToLower() == id);
            if (node == null)
                return false;

            SelectedNode.DeleteConnection(node);

            return true;
        }

        public static INode? CreateNodeByName(string name)
        {
            INode? newNode = GetAllNodes().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

            if (newNode != null)
                SetCreatedNode(newNode);

            return newNode;

        }

        public static void SetCreatedNode(INode node)
        {
            if (!CreatedNodes.Contains(node))
            {
                CreatedNodes.Add(node);
            }
        }

        public static List<INode> GetAllNodes()
        {
            List<INode> nodes = new List<INode>();
            var assembly = Assembly.GetExecutingAssembly();

            // Get all types in the assembly
            var allTypes = assembly.GetTypes();

            // Filter for classes that can be instantiated (non-abstract and public)
            var nodeTypes = allTypes.Where(t => typeof(INode).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            foreach (var type in nodeTypes)
            {
                var instance = (INode)Activator.CreateInstance(type, new object?[] { null })!;
                nodes.Add(instance);
            }

            return nodes;
        }

        public enum ArgumentType
        {
            BASE,
            VARIABLE,
            NONE,
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

        public static List<Argument> ExtractAnyArguments(this string cmd, List<string> noneArguments, string cmdBaseName)
        {
            string[] arrArgs = cmd.Split(" ");

            List<Argument> argResult = new List<Argument>();
            int paramIndex = 0;

            while(paramIndex < arrArgs.Length)
            {
                string argeKey = arrArgs[paramIndex].Trim().ToLower();

                if(argeKey.Length == 0 || argResult.Select(x => x.Key).Contains(argeKey))
                {
                    paramIndex++;
                    continue;
                }

                if(paramIndex == 0)
                {
                    string? argValue = null;
                    if (paramIndex + 1 < arrArgs.Length)
                    {
                        argValue = arrArgs[paramIndex + 1].Trim().ToLower();

                        if (argValue.StartsWith("--") || argValue.Length == 0 || !noneArguments.Contains(argValue))
                        {
                            argValue = null;
                        }else
                        {
                            paramIndex++;
                        }
                    }
                    else
                    {
                        argValue = null;
                    }
                    if(argValue != null)
                        argResult.Add(new Argument(ArgumentType.BASE, argeKey, argValue));
                }
                //arguments
                else if (argeKey.StartsWith("--"))
                {
                    string? argValue = null;
                    if (paramIndex + 1 < arrArgs.Length)
                    {
                        argValue = arrArgs[paramIndex+1].Trim().ToLower();
                        if (argValue.StartsWith("--") || argValue.Length == 0)
                        {
                            argValue = null;
                        }else
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

                    if (!noneArguments.Contains(argValue))
                    {
                        argValue = null;
                    }else
                    {
                        paramIndex++;
                    }
                    argResult.Add(new Argument(ArgumentType.NONE, argeKey, argValue));
                }

                paramIndex++;
            }
/*

            foreach (var a in arrArgs)
            {
                string[] argValue = a.Trim().Split(' ');

                if(argValue[0].Trim().Length > 0)
                {
                    if (argValue.Length == 2)
                        result[argValue[0].Trim()] = argValue[1].Trim();
                    else
                        result[argValue[0].Trim()] = null;
                }
              
            }*/

            return argResult;
        }


        public static Dictionary<string, string>? ExtractArguments(this string cmd, string cmdBaseName, List<CliArgument>? args)
        {
            if (args == null)
                return null;
            string argsText = cmd.Substring(cmdBaseName.Length);
            string[] arrArgs = argsText.Split("--");

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var a in arrArgs)
            {
                string[] argValue = a.Trim().Split(' ');

                var foundArg = args.FirstOrDefault(x => x.Name == argValue[0]);

                if (foundArg != null)
                {
                    if(foundArg.StaticValue != null)
                    {
                        result[foundArg.Name] = foundArg.StaticValue;
                    }
                    else if(argValue.Length == 2)
                        result[foundArg.Name] = argValue[1];
                    else if(foundArg.Default != null)
                        result[foundArg.Name] = foundArg.Default;

                }
            }

            return result;
        }

        public static Dictionary<int, string>? ExtractNames(string cmd)
        {
            string[] texts = cmd.Split("-");

            Dictionary<int, string>? names = new();
            int index = 0;
            foreach (var t in texts)
            {
                string txt = t.Trim();
                if(txt.StartsWith('[') && txt.EndsWith(']'))
                {
                    names[index] = txt.Substring(1, txt.Length - 2);
                }

                index++;
            }

            return names;
        }
    }
}
