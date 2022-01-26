
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
            const int r = 5;
            bool sayHiComplete = false;
            int hp = 10;
            int orcHp = 20;
            int distanceBetweenOrc = 10;
            int distanceBetweenGoblin = 12;

            tree.Builder
                .Repeat()
                    .Select()
                        .Sequence()
                            .Do()
                                .Start(() => Console.WriteLine("far way 5r from orc"))
                                .GetResult(() =>
                                {
                                    if (distanceBetweenOrc <= 5 * r)
                                        return Node.NodeResult.Success;
                                    return Node.NodeResult.Failure;
                                })
                            .End()
                            .Do()
                                .Start(() => Console.WriteLine("Hp is low?"))
                                .GetResult(() =>
                                {
                                    if (hp < 5)
                                        return Node.NodeResult.Success;
                                    return Node.NodeResult.Failure;
                                })
                            .End()
                            .Do()
                                .Start(() => Console.WriteLine("Yes ,Hp is low"))
                                .Update(() =>
                                {
                                    distanceBetweenGoblin++;
                                    distanceBetweenOrc++;
                                    ShowDetails();
                                })
                                .GetResult(() =>
                                {
                                    return Node.NodeResult.Success;
                                })
                            .End()
                        .End()
                        .Sequence()
                            .Do()
                                .Start(()=>Console.WriteLine("Orc is alive?"))
                                .GetResult(() =>
                                {
                                    if (orcHp > 0)
                                        return Node.NodeResult.Success;
                                    return Node.NodeResult.Failure;
                                })
                            .End()
                            .Do()
                                .Start(() => Console.WriteLine("Orc is near?"))
                                .GetResult(() =>
                                {
                                    if (distanceBetweenOrc < r)
                                        return Node.NodeResult.Success;
                                    return Node.NodeResult.Failure;
                                })
                            .End()
                            .Do()
                                .Start(() => Console.WriteLine("Yes, Orc is near"))
                                .Update(() =>
                                {
                                    hp--;
                                    orcHp--;
                                    ShowDetails();
                                })
                                .GetResult(() =>
                                {
                                    return Node.NodeResult.Success;
                                })
                            .End()
                        .End()
                        .Sequence()
                            .Do()
                                .Start(() => Console.WriteLine("Goblin is near?"))
                                .GetResult(() =>
                                {
                                    if (distanceBetweenGoblin < r)
                                        return Node.NodeResult.Success;
                                    return Node.NodeResult.Failure;
                                })
                            .End()
                            .Do()
                                .Start(() => Console.WriteLine("Has said hi?"))
                                .GetResult(() =>
                                {
                                    if (!sayHiComplete)
                                        return Node.NodeResult.Success;
                                    return Node.NodeResult.Failure;
                                })
                            .End()
                            .Do()
                                .Start(() => Console.WriteLine("Say Hi"))
                                .Update(() =>
                                {
                                    sayHiComplete = true;
                                    Console.WriteLine("Say Hi to Gobin!!");
                                    ShowDetails();
                                })
                                .GetResult(() => Node.NodeResult.Success)
                            .End()
                        .End()
                        .Sequence()
                            .Do()
                                .Start(() => Console.WriteLine("hp++"))
                                .Update(() =>
                                {
                                    distanceBetweenGoblin--;
                                    distanceBetweenOrc--;
                                    hp++;
                                    ShowDetails();
                                })
                                .GetResult(() => Node.NodeResult.Success)
                            .End()
                        .End()
                    .End()
                .End();

                                
                                
                    






            void ShowDetails()
            {
                Console.WriteLine($"Hp {hp} OrcHp {orcHp} DistOrc {distanceBetweenOrc} DistGobin {distanceBetweenGoblin} Has Say Hi {sayHiComplete}");
            }

            while (true)
            {
                tree.Execute();
                Thread.Sleep(100);
            }
        }
    }
}
