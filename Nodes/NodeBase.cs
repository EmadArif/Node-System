using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Nodes
{

    public abstract class NodeBase : INode
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        public abstract Type[]? InputTypes { get; }
        public abstract Type[]? OutputTypes { get; }
        public int Id { get; set; }
        public List<INode> Inputs { get; set; } = new List<INode>();
        public List<INode> Outputs { get; set; } = new List<INode>();
        public Func<Type, Task<Type>>? Process { get; set; }
        public Func<object, INode, Task>? OnExecute { get; set; }
        public virtual List<INode> AllConnectedNodes
        {
            get
            {
                return Outputs;
            }
        }
        public NodeBase(string name = "")
        {
            Name = name;
        }

        public void AddConnection(params INode[] nodes)
        {
            foreach (var n in nodes)
            {
                if (n.InputTypes == null)
                    continue;

                if (IsValidOutputType(n.InputTypes))
                {
                    Outputs.Add(n);
                }
            }

        }

        //public void AddInput(params INode[] nodes)
        //{
        //    foreach(var n in nodes)
        //    {
        //        if (n.OutputTypes == null)
        //            continue;

        //        ValidateInputType(n.OutputTypes);
        //        Inputs.Add(n);
        //    }

        //}

        public virtual async Task Execute(object input)
        {
            await Task.Delay(500);
            ValidateInputValue(input);
            if (Process == null)
                return;
        }

/*        private void ValidateInputType(Type[] types)
        {
            foreach (var n in types)
            {
                if (n == typeof(object))
                    return;

                if (!InputTypes!.Contains(n))
                {
                    throw new InvalidOperationException("Invalid input type.");
                }
            }
        }*/

        public bool IsValidOutputType(Type[]? types)
        {
            if (types == null)
                return false;

            bool found = false;

            foreach (var n in types)
            {
                if (n == typeof(object))
                {
                    found = true;
                    break;
                }

                if (OutputTypes!.Contains(n))
                {
                    found = true;
                    break;
                }
            }

          /*  if (!found)
            {
                throw new InvalidOperationException("Invalid input type.");
            }*/

            return found;
        }

        public void ValidateInputValue(object? input)
        {
            if (input == null)
                return;

            if (InputTypes == null)
                return;
            if (InputTypes.Contains(typeof(object)))
                return;

            if (!InputTypes.Contains(input.GetType()))
            {
                throw new InvalidOperationException("Invalid input type.");
            }
        }
    }
}
