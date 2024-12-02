using NodeSystem.Commands;
using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem
{
    public class SimpleUserSystem
    {
        private INode? selectedNode = null;
        private readonly List<INode> allNodes = new List<INode>();

        public async Task Execute()
        {

            TextNode starterText = new TextNode("starter text");
            TextNode text1 = new TextNode("text1");
            TextNode text2 = new TextNode("text2");
            TextNode text3 = new TextNode("text3");
            CounterNode counter = new CounterNode("counter");
            ConditionNode condition1 = new ConditionNode("condition 1");
            ConditionNode condition2 = new ConditionNode("condition 2");

            allNodes.Add(text1);
            allNodes.Add(text2);
            allNodes.Add(text3);
            allNodes.Add(condition1);
            allNodes.Add(condition2);

            //Override the Process function.
            counter.Process = async (v) =>
            {
                bool parsed = int.TryParse(v.ToString(), out int value);
                if (parsed)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(20);
                        Console.WriteLine(i);
                    }

                    return value;
                }
                return 0;
            };

            condition1.Process = (v) =>
            {
                Console.Write("Enter Condition 1 True/False: ");
                string? value = Console.ReadLine();


                if (value == "True")
                    return Task.FromResult(true);

                return Task.FromResult(false);
            };
            condition2.Process = (v) =>
            {
                Console.Write("Enter Condition 2 True/False: ");
                string? value = Console.ReadLine();


                if (value == "True")
                    return Task.FromResult(true);

                return Task.FromResult(false);
            };

            CmdManager.Init();
            while (true)
            {

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("CMD>");
                string? cmdText = Console.ReadLine();
                cmdText = cmdText.ToLower();
                Console.ForegroundColor = ConsoleColor.White;
                var (args, cmds) = CmdManager.ExtractCommands(cmdText);


                if(cmds != null && cmds.Count > 0)
                {
                    foreach (var d in cmds)
                    {
                        
                        d.Execute(args);
                    }
                }

                continue;
                ICommand? selectedCmd = null;

                foreach(var cmd in CmdManager.Commands)
                {
                    if (cmd.ValidateName(cmdText!.Trim().ToLower()))
                    {
                        selectedCmd = cmd;
                        break;
                    }
                }

                if (selectedCmd == null)
                    continue;
                
                selectedCmd.Execute(cmdText!);

            }
        }
/*
        public void DisplayCommands(string cmd)
        {
            string lowerCmd = cmd.ToLower();

            if (lowerCmd == "?" || lowerCmd == "help")
            {
                Console.WriteLine("For more information on a specific command, type HELP command-name");
                TypeCmdInfo("nodes", "Display all available nodes.");
                TypeCmdInfo("select-node-[Number]", "Set selected node by it's number.");
                TypeCmdInfo("connect", "Connect selected node with other nodes");
                TypeCmdInfo("delete", "Delete connected nodes from the selected node.");
                TypeCmdInfo("draw", "Draw the full path of the selected node.");
                return;
            }
            if(lowerCmd == "nodes")
            {
                Console.WriteLine("-----------------");

                for (int i = 0; i < allNodes.Count; i++)
                {
                    Console.WriteLine(i + "-" + allNodes[i].Name.ToUpper());
                }
                Console.WriteLine("-----------------");
                return;
            }
            if (lowerCmd.StartsWith("select-node-[") && lowerCmd.EndsWith(']'))
            {
                int? value = CmdManager.ExtractValowerCmd);
                if(value != null && value >= 0 && value < allNodes.Count)
                {
                    selectedNode = allNodes[value ?? -1];
                    Console.WriteLine(selectedNode.Name.ToUpper() + " is selected");
                }
                return;
            }
            if(lowerCmd == "connect")
            {
                if(selectedNode != null)
                {
                    Console.WriteLine($"List the node numbers to connect them to {selectedNode.Name.ToUpper()}, Enter exit to return.");
                    string? nn;
                    do
                    {
                        nn = Console.ReadLine();
                        bool parsed = int.TryParse(nn, out int number);
                        if (parsed && number >= 0 && number < allNodes.Count)
                        {
                            selectedNode.AddConnection(allNodes[number]);
                        }

                    } while (nn.ToLower() != "exit");
                    new NodeManager().DrawConnectedNodes(selectedNode);

                }
                else
                {
                    Console.WriteLine("There is node selected node!");
                }

                return;
            }
            if (lowerCmd == "draw" )
            {
                if(selectedNode != null)
                    new NodeManager().DrawConnectedNodes(selectedNode);
                else
                    Console.WriteLine("There is node selected node!");

                return;
            }
            if (lowerCmd == "delete")
            {
                if (selectedNode != null)
                {
                    Console.WriteLine($"List the node numbers to delete them from {selectedNode.Name.ToUpper()}, Enter exit to return.");
                    string? nn;
                    do
                    {
                        nn = Console.ReadLine();
                        bool parsed = int.TryParse(nn, out int number);
                        if (parsed && number >= 0 && number < allNodes.Count)
                        {
                            selectedNode.DeleteConnection(allNodes[number]);
                        }

                    } while (nn.ToLower() != "exit");
                    new NodeManager().DrawConnectedNodes(selectedNode);
                }
                else
                    Console.WriteLine("There is node selected node!");


                return;
            }

        }
*/
        
        private void TypeCmdInfo(string cmdName, string description)
        {
            Console.Write(cmdName.ToUpper().PadRight(40));
            Console.WriteLine(description);

        }
    }
}
