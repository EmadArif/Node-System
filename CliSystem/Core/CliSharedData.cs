using NodeSystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.CliSystem.Core
{
    public class CliSharedData
    {
        public INode? SelectedNode { get; private set; }
        public readonly List<INode> CreatedNodes = new();

        private List<INode>? _nodes = null;
        public List<INode> Nodes 
        {   get
            {
                _nodes = null;
                if (_nodes == null)
                {
                    _nodes = new List<INode>();
                    var assembly = Assembly.GetExecutingAssembly();

                    // Get all types in the assembly
                    var allTypes = assembly.GetTypes();

                    // Filter for classes that can be instantiated (non-abstract and public)
                    var nodeTypes = allTypes.Where(t => typeof(INode).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
                    foreach (var type in nodeTypes)
                    {
                        var instance = (INode)Activator.CreateInstance(type, new object?[] { null })!;
                        _nodes.Add(instance);
                    }

                }
                return _nodes ?? new List<INode>();
            } 
        }

        public void SetSelectedNode(INode node)
        {
            SelectedNode = node;
        }

        public void SetCreatedNode(INode node)
        {
            if (!CreatedNodes.Contains(node))
            {
                CreatedNodes.Add(node);
            }
        }

        public bool DeleteCreatedNode(INode node)
        {
            CreatedNodes.ForEach(x => x.DeleteConnection(node));
            return CreatedNodes.RemoveAll(x => x.Guid.ToString().ToLower() == node.Guid.ToString().ToLower()) > 0;
        }

        public bool DeleteSelectedNodeConnection(INode node)
        {
            if (SelectedNode == null)
                return false;

            SelectedNode.DeleteConnection(node);

            return true;
        }
    }
}
