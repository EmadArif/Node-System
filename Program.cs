// See https://aka.ms/n;ew-console-template for more information
using NodeSystem;



/*

List<INode> allNodes = new List<INode>();

TextNode starterText = new TextNode("starter text");
TextNode text1 = new TextNode("text1");
TextNode text2 = new TextNode("text2");
TextNode text3 = new TextNode("text3");
CounterNode counter = new CounterNode("counter");
ConditionNode condition1 = new ConditionNode("condition 1");
ConditionNode condition2 = new ConditionNode("condition 2");

allNodes.Add(text1);
allNodes.Add(text2);
allNodes.Add(text3);
allNodes.Add(condition1);
allNodes.Add(condition2);

//Override the Process function.
counter.Process = async (v) =>
{
    bool parsed = int.TryParse(v.ToString(), out int value);
    if (parsed)
    {
        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(20);
            Console.WriteLine(i);
        }

        return value;
    }
   return 0;
};

condition1.Process = (v) =>
{
    Console.Write("Enter Condition 1 True/False: ");
    string? value = Console.ReadLine();


    if (value == "True")
        return Task.FromResult(true);

    return Task.FromResult(false);
};
condition2.Process = (v) =>
{
    Console.Write("Enter Condition 2 True/False: ");
    string? value = Console.ReadLine();


    if (value == "True")
        return Task.FromResult(true);

    return Task.FromResult(false);
};

//Connect the nodes.
starterText.AddConnection(condition1);

condition1.AddTrueNode(text1);
condition1.AddTrueNode(text2);
condition1.AddTrueNode(text3);

condition1.AddFalseNode(condition2);

condition2.AddTrueNode(text2);
condition2.AddTrueNode(text2);
condition2.AddTrueNode(text1);

condition2.AddTrueNode(text1);

text1.AddConnection(counter);
text2.AddConnection(text3);
text3.AddConnection(text1);



NodeManager nodeManager = new();
nodeManager.IsLoop = true;
nodeManager.AddNode(starterText);
nodeManager.AddNode(condition1);
nodeManager.AddNode(condition2);
nodeManager.AddNode(text1);
nodeManager.AddNode(text2);
nodeManager.AddNode(text3);
*/

/*
await nodeManager.Execute("Start", async (value, node) =>
{
    if (node is ConditionNode)
        return;

    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write(node.Name + ": ");
    Console.ForegroundColor = ConsoleColor.White;

    Console.WriteLine(value);
});*/
CliExecuter s = new CliExecuter();
await s.Execute();

Console.ReadKey();

