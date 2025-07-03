
using System;
using System.Threading;

namespace Task.Switch.Structure.BT
{
    class Program
    {
        class Data
        {
            public int Id;
            public string Name;
            public int Count;
            public Data(int id, string name, int count)
            {
                Id = id;
                Name = name;
                Count = count;
            }
            public override string ToString()
            {
                return $"Data:{Id} {Name} {Count}";
            }
            
        }
        static void Main(string[] args)
        {
            Console.WriteLine("MAIN...");
            BehaviourTree<Data> tree = new BehaviourTree<Data>(new Data(100, "name", 0));

            tree
                .Repeat()
                    .Select()
                        .Do()
                            .Start(data => { Console.WriteLine($"START EAT APPLE");data.Count = 0; })
                            .Update(data => { Console.WriteLine($"EATING APPLE");data.Count++; })
                            .Stop(data => Console.WriteLine($"STOP EAT APPLE"))
                            .SetResult(data => 
                            {
                                if (data.Count < 5) return NodeResult.Continue;
                                else return NodeResult.Failure;
                            })
                        .End()
                        .Do()
                            .Start(data => { Console.WriteLine($"START EAT ORANGE"); data.Count = 0; })
                            .Update(data => { Console.WriteLine($"EATING ORANGE"); data.Count++; })
                            .Stop(data => Console.WriteLine($"STOP EAT ORANGE"))
                            .SetResult(data =>
                            {
                                if(data.Count < 6) return NodeResult.Continue;
                                else return NodeResult.Failure;
                            })
                        .End()
                        .Sequence()
                            .Do()
                                .Start(data => { Console.WriteLine("START PEEL BANANA"); data.Count = 0; })
                                .Update(data => { Console.WriteLine($"PEELING BANANA"); data.Count++; })
                                .Stop(data => Console.WriteLine($"STOP PEEL BANANA"))
                                .SetResult(data =>
                                {
                                    if (data.Count < 3) return NodeResult.Continue;
                                    else return NodeResult.Success;
                                })
                            .End()
                            .Do()
                                .Start(data => { Console.WriteLine("START EAT BANANA"); data.Count = 0; })
                                .Update(data => { Console.WriteLine($"EATING BANANA"); data.Count++; })
                                .Stop(data => Console.WriteLine($"STOP EAT BANANA"))
                                .SetResult(data =>
                                {
                                    if(data.Count < 4) return NodeResult.Continue;
                                    else return NodeResult.Success;
                                })
                            .End()
                        .End()
                    .End()
                .End();


            while (true)
            {
                tree.Execute();
                Thread.Sleep(100);
            }
        }
    }
}
