
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
            BehaviourTree<Data> tree = new BehaviourTree<Data>(new Data(100, "name", 90));

            tree.Builder
                .Repeat()
                    .Sequence()
                        .Do()
                            .Start(data => Console.WriteLine($"Start1 {data}"))
                            .Update(data => Console.WriteLine($"Update1 {data}"))
                            .Stop(data => Console.WriteLine($"Stop1 {data}"))
                            .GetResult(data => NodeResult.Success)
                        .End()
                        .Do()
                            .Start(data => Console.WriteLine($"Start2 {data}"))
                            .Update(data => Console.WriteLine($"Update2 {data}"))
                            .Stop(data => Console.WriteLine($"Stop2 {data}"))
                            .GetResult(data => NodeResult.Success)
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
