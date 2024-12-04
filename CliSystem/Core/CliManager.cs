using NodeSystem.CliSystem.Core;
using NodeSystem.Commands.Data;
using NodeSystem.Nodes;

namespace NodeSystem.Commands
{
    public static class CliManager
    {

        public readonly static CliSharedData Shared = new();


        public static void SetSelectedNode(INode node)
        {
            Shared.SetSelectedNode(node);
            Shared.SetCreatedNode(node);
        }
        public static INode? CreateNodeByName(string name)
        {
            INode? newNode = Shared.Nodes.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

            if (newNode != null)
            {
                Shared.SetCreatedNode(newNode);
            }

            return newNode;

        }

        public static bool DeleteCreatedNodeById(string id)
        {
            INode? node = Shared.CreatedNodes.SingleOrDefault(t => t.Guid.ToString().ToLower() == id);
            if (node == null)
                return false;

            return Shared.DeleteCreatedNode(node);
        }

        public static bool DeleteConnectionFromSelected(string id)
        {
            if (Shared.SelectedNode == null)
                return false;

            INode? node = Shared.CreatedNodes.SingleOrDefault(t => t.Guid.ToString().ToLower() == id);
            if (node == null)
                return false;

            return Shared.DeleteSelectedNodeConnection(node);
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
    }
}
