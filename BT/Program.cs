using BT.Action;
using BT.Composite;
using BT.Decorator;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BT
{
    class Program
    {
        static void Main(string[] args)
        {
            BehaviourTree tree = new BehaviourTree();

            RepeatNode repeatNode = new RepeatNode();

            SequencerNode sequencerNode = new SequencerNode();
            tree.SetChild(repeatNode);
            repeatNode.Child = sequencerNode;
            DebugNode hungry = new DebugNode() { message = "Hungry?" };
            sequencerNode.Children.Add(hungry);

            DebugNode yesHungry = new DebugNode() { message = "yes , very hungry" }; 
            sequencerNode.Children.Add(yesHungry);

            SelectorNode select = new SelectorNode();
            
            select.Children.Add(new DebugNode() { message = "eat apple" });
            select.Children.Add(new DebugNode() { message = "eat banana"});

            sequencerNode.Children.Add(select);

            while (true)
            {
                tree.Execute();
                Thread.Sleep(1000);
            }
            Console.WriteLine("Hello World!");
        }
    }
}
