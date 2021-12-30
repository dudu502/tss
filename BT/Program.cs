using BT.Action;
using BT.Composite;
using BT.Decorator;
using System;
using System.Threading;

namespace BT
{
    class Program
    {
        static void Main(string[] args)
        {
            BehaviourTree tree = new BehaviourTree();
            RepeatNode repeat = new RepeatNode();

            SequencerNode sequencer = new SequencerNode();
            tree.SetChild(repeat);
            repeat.Child = sequencer;

            sequencer.Children.Add(new DebugNode() { message = "seq-001" });
            sequencer.Children.Add(new DebugNode() { message = "seq-002" });
            sequencer.Children.Add(new DebugNode() { message = "seq-003" });

            SelectorNode select = new SelectorNode();
            select.Children.Add(new DebugNode() { message = "select-001" });
            select.Children.Add(new DebugNode() { message = "select-002" });
            select.Children.Add(new DebugNode() { message = "select-003" });
            sequencer.Children.Add(select);
            ParallelNode parallel = new ParallelNode();

            parallel.Children.Add(new DebugNode() { message = "par-001", State = Node.NodeResult.Success });
            parallel.Children.Add(new DebugNode() { message = "par-002", State = Node.NodeResult.Success });
            parallel.Children.Add(new DebugNode() { message = "par-003", State = Node.NodeResult.Success });
            sequencer.Children.Add(parallel);

            while (true)
            {
                tree.Execute();
                Thread.Sleep(1000);
            }

            Console.WriteLine("Hello World!");
        }
    }
}
