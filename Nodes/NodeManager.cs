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
        public bool CheckHasInfiniteLoop(INode node)
        {
            return HasInfiniteLoop(node);
        }

        private bool HasInfiniteLoop(INode node, int depth = 0, Dictionary<Guid, int>? visitedNodes = null, HashSet<Guid>? activeNodes = null)
        {
            // إنشاء القاموس والمجموعة عند الاستدعاء الأول
            visitedNodes ??= new Dictionary<Guid, int>();
            activeNodes ??= new HashSet<Guid>();

            // معرف العقدة الحالية
            var nodeGuid = node.Guid;

            // التحقق من الحلقات النهائية
            if (activeNodes.Contains(nodeGuid))
            {
                return true; // حلقة نهائية مكتشفة
            }

            // تسجيل العقدة في العقد النشطة
            activeNodes.Add(nodeGuid);

            // إذا كانت العقدة جديدة، أضفها إلى العقد التي تمت زيارتها
            if (!visitedNodes.ContainsKey(nodeGuid))
            {
                visitedNodes[nodeGuid] = depth;
            }

            // معالجة العقد المتصلة بناءً على نوع العقدة
            if (node is ConditionNode condNode)
            {
                // التحقق من العقد المتصلة في حالة True
                foreach (INode trueNode in condNode.TrueNodes)
                {
                    if (HasInfiniteLoop(trueNode, depth + 1, visitedNodes, activeNodes))
                        return true;
                }

                // التحقق من العقد المتصلة في حالة False
                foreach (INode falseNode in condNode.FalseNodes)
                {
                    if (HasInfiniteLoop(falseNode, depth + 1, visitedNodes, activeNodes))
                        return true;
                }
            }
            else
            {
                // التحقق من المخرجات العادية
                foreach (INode outputNode in node.Outputs)
                {
                    if (HasInfiniteLoop(outputNode, depth + 1, visitedNodes, activeNodes))
                        return true;
                }
            }

            // إزالة العقدة من العقد النشطة بعد المعالجة
            activeNodes.Remove(nodeGuid);

            return false; // لا توجد دورة نهائية
        }

        public void DrawConnectedNodes(INode node, bool details = false)
        {
            DetectInfiniteLoop(node, details);
        }

        private bool DetectInfiniteLoop(INode node, bool details = false, int depth = 0, Dictionary<Guid, int>? visitedNodes = null, HashSet<Guid>? activeNodes = null)
        {
            // إنشاء القاموس والمجموعة عند الاستدعاء الأول
            if (visitedNodes == null)
            {
                visitedNodes = new Dictionary<Guid, int>();
            }
            if (activeNodes == null)
            {
                activeNodes = new HashSet<Guid>();
            }

            // تحويل معرف العقدة الحالية إلى سلسلة نصية
            var nodeGuid = node.Guid;

            // التحقق من الحلقات النهائية
            if (activeNodes.Contains(nodeGuid))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: Final Loop Detected! Node '{node.Name}' is revisiting itself in the same path.");
                Console.ForegroundColor = ConsoleColor.White;

                return true;
            }

            // إضافة العقدة إلى مجموعة العقد النشطة
            activeNodes.Add(nodeGuid);

            // تحقق ما إذا كانت العقدة قد زارت مسبقاً
            if (!visitedNodes.TryGetValue(nodeGuid, out _))
            {
                visitedNodes[nodeGuid] = depth;
            }

            // طباعة اسم العقدة
            GenerateSpaces(depth);
            Console.Write("-");
            if (details)
                Console.WriteLine(node.Name.PadRight(10) + node.Guid);
            else
                Console.WriteLine(node.Name);

            // معالجة العقدة حسب نوعها
            if (node is ConditionNode condNode)
            {
                // العقد المتصلة في حالة الشرط True
                GenerateSpaces(depth);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("---TRUE:");
                Console.ForegroundColor = ConsoleColor.White;

                foreach (INode n in condNode.TrueNodes)
                {
                    if (DetectInfiniteLoop(n, details, depth + 1, visitedNodes, activeNodes))
                        return true;
                }

                // العقد المتصلة في حالة الشرط False
                GenerateSpaces(depth);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("---FALSE:");
                Console.ForegroundColor = ConsoleColor.White;

                foreach (INode n in condNode.FalseNodes)
                {
                    if (DetectInfiniteLoop(n, details, depth + 1, visitedNodes, activeNodes))
                        return true;
                }

            }
            else
            {
                // العقد المتصلة عبر المخرجات
                foreach (INode n in node.Outputs)
                {
                    if (DetectInfiniteLoop(n, details, depth + 1, visitedNodes, activeNodes))
                        return true;
                }
            }

            // إزالة العقدة من العقد النشطة بعد اكتمال المعالجة
            activeNodes.Remove(nodeGuid);

            return false; // لم يتم العثور على دورة نهائية
        }

        private void GenerateSpaces(int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                if (i == depth - 1)
                {
                    Console.Write(" |---");

                }
                else
                    Console.Write(" |   ");
            }
        }

        public async Task Execute(object input, Func<object, INode, Task>? onExecute = null)
        {
            if (AllNodes.Count < StarterNodeIndex || AllNodes.Count == StarterNodeIndex)
                return;
            var starterNode = AllNodes[StarterNodeIndex];

            if (CheckHasInfiniteLoop(starterNode))
            {
                DrawConnectedNodes(starterNode);
                return;
            }

            if(onExecute != null)
            {
                foreach (INode node in AllNodes)
                {
                    node.OnExecute = onExecute;
                }
            }


            if (starterNode == null)
                return;

            do
            {
                await starterNode.Execute(input);
            }
            while (IsLoop);
            Console.WriteLine("End");
        }

    }
}
