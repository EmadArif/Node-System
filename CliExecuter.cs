using NodeSystem.CliSystem.Core;
using NodeSystem.Commands.Base;
using NodeSystem.Nodes;


namespace NodeSystem
{
    public class CliExecuter
    {
        public async Task Execute()
        {
            while (true)
            {
                string? cmdText = CliProcessor.ProcessInputs();
                cmdText = cmdText.ToLower();
                var (args, cmds) = CliProcessor.ExtractCommands(cmdText);


                if(cmds != null && cmds.Count > 0)
                {
                    foreach (var d in cmds)
                    {
                        d.Execute(args);
                    }
                }

                continue;
            

            }
        }
    }
}
