using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Nodes
{

    public class ConditionNode : NodeBase
    {
        override public Type[] InputTypes { get; } = new Type[] { typeof(object) };
        override public Type[] OutputTypes { get; } = new Type[] { typeof(object) };
        public new Func<object, Task<bool>>? Process { get; set; }

        public readonly List<INode> TrueNodes = new();
        public readonly List<INode> FalseNodes = new();
        public override List<INode> AllConnectedNodes
        { get
            {
                List<INode> nodes = new List<INode>();
                nodes.AddRange(TrueNodes);
                nodes.AddRange(FalseNodes);
                return nodes;
            }
        }

        public ConditionNode(string? name) : base(name ?? "Condition")
        {

        }

        public void AddTrueNode(INode node)
        {
            TrueNodes.Add(node);
            AddConnection(node);
        }
        public void AddFalseNode(INode node)
        {
            FalseNodes.Add(node);
            AddConnection(node);
        }
        public override async Task Execute(object input)
        {
            await base.Execute(input);

            bool status = await Process!(input);

            if (OnExecute != null)
            {
                await OnExecute(input, this);
            }

            await Task.Delay(500);

            if (status)
            {
                foreach (var node in TrueNodes)
                {
                    await node.Execute(input);
                }
            }
            else
            {
                foreach (var node in FalseNodes)
                {
                    await node.Execute(input);
                }
            }
        }
    }
}
