
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
                                        .Update(() => Console.WriteLine("Update Hungry?"))
                                        .Stop(() => Console.WriteLine("Stop Hungry?"))
                                        .End()
                                    .Do()
                                        .Start(() => Console.WriteLine("Yes, Start Very Hungry"))
                                        .Update(() => Console.WriteLine("Yes, Update Very Hungry"))
                                        .Stop(() => Console.WriteLine("Yes, Stop Very Hungry"))
                                        .End()
                                    .Select()
                                        .WaitFrame(3, Node.NodeResult.Failure)
                                            .Update(() => Console.WriteLine("Think ............."))
                                            .End()
                                        .Do()
                                            .Start(() => Console.WriteLine("Start Eat Apple"))
                                            .Update(() => Console.WriteLine("Update Eat Apple"))
                                            .GetResult(() => Node.NodeResult.Failure)
                                            .Stop(() => Console.WriteLine("Stop Eat Apple"))
                                            .End()
                                        .Do()
                                            .Start(() => Console.WriteLine("Start Eat Banana"))
                                            .Update(() => Console.WriteLine("Update Eat Banana"))
                                            .GetResult(() => Node.NodeResult.Success)
                                            .Stop(() => Console.WriteLine("Stop Eat Banana"))
                                            .End()
                                        .End()
                                    .Do()
                                        .Start(() => Console.WriteLine("Finish Start Eat "))
                                        .Update(() => Console.WriteLine("Finish Update End"))
                                        .Stop(()=>Console.WriteLine("Finish Stop End"))
                                        .End();
                            
            
            while (true)
            {
                tree.Execute();
                Thread.Sleep(500);
            }
        }
    }
}
