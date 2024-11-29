using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Nodes
{

    public interface INode
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public Type[]? InputTypes { get; }
        public Type[]? OutputTypes { get; }
        public int Id { get; set; }
        public List<INode> Inputs { get; set; }
        public List<INode> Outputs { get; set; }
        public Func<Type,Task<Type>>? Process { get; set; }
        public Func<object, INode, Task>? OnExecute { get; set; }
        public virtual List<INode> AllConnectedNodes
        {
            get
            {
                return Outputs;
            }
        }

        public Task Execute(object input);
    }
}
