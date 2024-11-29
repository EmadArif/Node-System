// See https://aka.ms/new-console-template for more information
using NodeSystem.Nodes;


TextNode text1 = new TextNode("text1");
TextNode text2= new TextNode("text2");
TextNode text3 = new TextNode("text3");

CounterNode counter = new CounterNode("counter");
counter.Process = async (v) =>
{
    for (int i = 0; i < 10; i++)
    {
        await Task.Delay(20);
        Console.WriteLine(i);
    }
    return 33;
};

ConditionNode condition1 = new ConditionNode("condition 1");


condition1.AddTrueNode(text1);

condition1.AddFalseNode(text2);
condition1.AddFalseNode(text1);

condition1.Process = (v) =>
{
    Console.Write("Enter True/False: ");
    string? value = Console.ReadLine();


    if (value == "True")
        return Task.FromResult(true);

    return Task.FromResult(false);
};


ConditionNode condition2 = new ConditionNode("condition 2");

condition2.AddTrueNode(text2);
condition2.AddTrueNode(text2);
condition2.AddTrueNode(text1);

text1.AddConnection(counter);
text2.AddConnection(text3);
text3.AddConnection(text1);

ConditionNode condition3 = new ConditionNode("condition 3");

condition3.AddTrueNode(text2);
condition3.AddTrueNode(text2);
condition3.AddTrueNode(text1);

condition2.AddTrueNode(condition3);


NodeManager nodeManager = new();
/*nodeManager.IsLoop = true;
nodeManager.AddNode(condition1);
nodeManager.AddNode(text1);
nodeManager.AddNode(text2);
nodeManager.AddNode(counter);
*/


nodeManager.DrawConnectedNodes(condition2);
/*
await nodeManager.Execute("Hello Friend", async (value,node) =>
{
    if (node is ConditionNode)
        return;

    Console.WriteLine(node.Name + ": " + value + " == " + node.Guid.ToString());
});*/


Console.ReadKey();

