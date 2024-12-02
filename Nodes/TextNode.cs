using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Nodes
{


    public class TextNode : NodeBase
    {


        public override Type[] InputTypes { get; } = new Type[] { typeof(object) };
        public override Type[] OutputTypes { get; } = new Type[] { typeof(string) };

        public new Func<object, Task<string>>? Process { get; set; } = (v) =>
        {
           
            return Task.FromResult(v.ToString());
        };
        public TextNode(string? name) : base(name ?? "Text")
        {

        }
        public override async Task Execute(object input)
        {
            await base.Execute(input);

            string? data = null;
            if(Process != null)
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
