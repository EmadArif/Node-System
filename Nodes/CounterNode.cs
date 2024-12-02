using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Nodes
{
    internal class CounterNode : NodeBase
    {

        public override Type[] InputTypes { get; } = new Type[] { typeof(int), typeof(string) };
        public override Type[] OutputTypes { get; } = new Type[] { typeof(int) };

        public new Func<object, Task<int>>? Process { get; set; } = (v) =>
        {
            int c = (int)v;
            return Task.FromResult(c);
        };

        public CounterNode(string? name) : base(name ?? "Counter")
        {

        }

        public override async Task Execute(object input)
        {
            await base.Execute(input);

            int? data = null;
            if (Process != null)
            {
                data = await Process(input);
            }

            if (OnExecute != null)
            {
                await OnExecute(input, this);
            }

            if (Outputs == null)
                return;

            foreach (var output in Outputs)
            {
                await output.Execute(data);
            }

        }
    }
}
