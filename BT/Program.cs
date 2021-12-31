
using System;
using System.Collections.Generic;
using System.Threading;

namespace Task.Switch.Structure.BT
{
    class Program
    {
        static void Main(string[] args)
        {
            BehaviourTree tree = new BehaviourTree();

            tree.Repeat()
                        .Sequence()
                                    .Do()
                                        .Start(() => Console.WriteLine("Start Hungry?"))
                                        .Update(() =>
                                        {
                                            Console.WriteLine("Update Hungry?");
                    
                                        })
                                        .Stop(() => Console.WriteLine("Stop Hungry?"))
                                        .End()
                                    .Do()
                                        .Start(() => Console.WriteLine("Yes,Start Very Hungry"))
                                        .Update(() =>
                                        {
                                            Console.WriteLine("Yes,Update Very Hungry");
                                   
                                        })
                                        .Stop(() => Console.WriteLine("Yes,Stop Very Hungry"))
                                        .End()
                                    .Select()
                                        .WaitFrame(1,Node.NodeResult.Failure)
                                            .End()
                                        .Do()
                                            .Start(() => Console.WriteLine("Start Eat apple"))
                                            .Update(() =>
                                            {
                                                Console.WriteLine("Update Eat apple");
                                     
                                            })
                                            .GetResult(() => Node.NodeResult.Failure)
                                            .Stop(() => Console.WriteLine("Stop Eat apple"))
                                            .End()
                                        .Do()
                                            .Start(() => Console.WriteLine("Start Eat banana"))
                                            .Update(() =>
                                            {
                                                Console.WriteLine("Update Eat banana");
                           
                                            })
                                            .Stop(() => Console.WriteLine("Stop Eat banana"))
                                            .End();

            while (true)
            {
                tree.Execute();
                Thread.Sleep(100);
            }
        }
    }
}
