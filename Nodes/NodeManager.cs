using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Nodes
{
    internal class NodeManager
    {

        public readonly List<INode> AllNodes = new List<INode>();
        public bool IsLoop = false;
        public int StarterNodeIndex { get; set; } = 0;
        public void AddNode(params INode[] node)
        {
            AllNodes.AddRange(node);
        }

        public IList<NodeBase> GetCompatibleNodes(NodeBase node)
        {
            List<NodeBase> compatibleNodes = new();

            foreach(NodeBase n in AllNodes)
            {
                if (node.IsValidOutputType(n.InputTypes))
                {
                    compatibleNodes.Add(n);
                }
            }

            return compatibleNodes;
        }

        public bool DrawConnectedNodes(INode node, int depth = 0, List<DepthData>? ids = null)
        {
             bool isInfinityLoop = false;

            if (isInfinityLoop)
                return true;

            string myGuid = node.Guid.ToString();
            foreach (var n in node.AllConnectedNodes)
            {
                myGuid += "(" + n.Guid+ ")";
            }

            if (ids == null)
            {
                ids = new List<DepthData>();
                ids.Add(new DepthData(myGuid, depth));
            }
            else if(myGuid.Length > 38)
            {
                DepthData foundNode = ids.FirstOrDefault(x => x.Depth < depth && x.Guids.Equals(myGuid));

                if(foundNode.Guids != null && foundNode.Guids.Equals(myGuid))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERROR: Infinity Loop Node: {node.Name}");
                    Console.ForegroundColor = ConsoleColor.White;

                    return true;
                }
                else
                {
                    ids.Add(new DepthData(myGuid, depth));
                }
            }

            GenerateSpaces(depth);

            Console.Write("-");

            Console.WriteLine(node.Name);


            if (node is ConditionNode condNode)
            {
                GenerateSpaces(depth);
                Console.WriteLine("----TRUE:");

                foreach (INode n in condNode.TrueNodes)
                {
                    isInfinityLoop = DrawConnectedNodes(n, depth + 1, ids);
                    if (isInfinityLoop)
                        break;
                }

                GenerateSpaces(depth);

                if (isInfinityLoop)
                    return true;

                Console.WriteLine("----FALSE:");

                foreach (INode n in condNode.FalseNodes)
                {
                    isInfinityLoop = DrawConnectedNodes(n, depth + 1, ids);
                    if (isInfinityLoop)
                        break;
                }
            }
            else
            {
                foreach (INode n in node.Outputs)
                {
                    isInfinityLoop = DrawConnectedNodes(n, depth + 1, ids);
                    if (isInfinityLoop)
                        break;
                }
            }
            return isInfinityLoop;
        }

        private void GenerateSpaces(int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                if (i == depth - 1)
                {
                    Console.Write("|---");

                }
                else
                    Console.Write("|   ");
            }
        }

        public async Task Execute(object input, Func<object, INode, Task>? onExecute = null)
        {
            if (AllNodes.Count < StarterNodeIndex || AllNodes.Count == StarterNodeIndex)
                return;

            if(onExecute != null)
            {
                foreach (INode node in AllNodes)
                {
                    node.OnExecute = onExecute;
                }
            }

            var starterNode = AllNodes[StarterNodeIndex];

            if (starterNode == null)
                return;

            do
            {
                await starterNode.Execute(input);
            }
            while (IsLoop);
            Console.WriteLine("End");
        }

        public struct DepthData
        {
            public string Guids;
            public int Depth;

            public DepthData(string guids, int depth)
            {
                Guids = guids;
                Depth = depth;
            }
            public static bool operator ==(DepthData p1, DepthData p2)
            {
                return p1.Guids == p2.Guids && p1.Depth == p2.Depth;
            }

            public static bool operator !=(DepthData p1, DepthData p2)
            {
                return !(p1 == p2);
            }
        }

    }
}
