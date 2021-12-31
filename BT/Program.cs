﻿using BT.Actions;
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

            tree.Repeat()
                        .Sequence()
                                    .Do()
                                        .Start(() => Console.WriteLine("Start Hungry?"))
                                        .Update(() =>
                                            {
                                                Console.WriteLine("Update Hungry?");
                                                return Node.NodeResult.Success;
                                            })
                                        .Stop(() => Console.WriteLine("Stop Hungry?"))
                                        .End()
                                    .Do()
                                        .Start(() => Console.WriteLine("Yes,Start Very Hungry"))
                                        .Update(() =>
                                        {
                                            Console.WriteLine("Yes,Update Very Hungry");
                                            return Node.NodeResult.Success;
                                        })
                                        .Stop(() => Console.WriteLine("Yes,Stop Very Hungry"))
                                        .End()
                                    .Select()
                                        .Wait(2000,Node.NodeResult.Failure)
                                            .End()
                                        .Do()
                                            .Start(() => Console.WriteLine("Start Eat apple"))
                                            .Update(() =>
                                            {
                                                Console.WriteLine("Update Eat apple");
                                                return Node.NodeResult.Success;
                                            })
                                            .Stop(() => Console.WriteLine("Stop Eat apple"))
                                            .End()
                                        .Do()
                                            .Start(() => Console.WriteLine("Start Eat banana"))
                                            .Update(() =>
                                            {
                                                Console.WriteLine("Update Eat banana");
                                                return Node.NodeResult.Success;
                                            })
                                            .Stop(() => Console.WriteLine("Stop Eat banana"))
                                            .End();

            while (true)
            {
                tree.Execute();
                Thread.Sleep(1000);
            }

            Console.WriteLine("Hello World!");
        }
    }
}
