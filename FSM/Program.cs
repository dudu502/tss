using System;
using System.Threading;

namespace Task.Switch.Structure.FSM
{
    class Program
    {
        enum State
        {
            SayHi,
            Patrol,
            Battle,
            Escape
        }
        static void Main(string[] args)
        {     
            // Create a state machine
            var machine = new StateMachine<State>();
            const int r = 5;
            bool sayHiComplete = false;
            int hp = 10;
            int orcHp = 20;
            int distanceBetweenOrc = 10;
            int distanceBetweenGoblin = 12;
            machine
                .NewState(State.Patrol)
                    .Initialize(() => Console.WriteLine("Init Patral State"))
                    .Enter(() => Console.WriteLine("Enter Patral State"))
                    .Update(() =>
                    {
                        distanceBetweenOrc--;
                        distanceBetweenGoblin--;
                        hp++;
                        Console.WriteLine($"Patral ING.... distanceBetweenOrc:{distanceBetweenOrc} distanceBetweenGobin:{distanceBetweenGoblin} Hp:{hp} orcHp:{orcHp}");
                    })
                    .Translate(() => distanceBetweenOrc < r && orcHp>0).To(State.Battle)
                    .Translate(() => distanceBetweenGoblin < r&&!sayHiComplete).To(State.SayHi)
                .End()
                .NewState(State.Battle)
                    .Initialize(() => Console.WriteLine("Init Battle State"))
                    .Enter(() => Console.WriteLine("Enter Battle State"))
                    .Update(() =>
                    {
                        hp--;
                        orcHp--;
                        Console.WriteLine("Fight ING.... Current HP " + hp+" ocrHp "+orcHp);
                    })
                    .Translate(() => hp < 5).To(State.Escape)
                    .Translate(()=> orcHp < 0).To(State.Patrol)
                .End()
                .NewState(State.Escape)
                    .Initialize(() => Console.WriteLine("Init Escape State"))
                    .Enter(() => Console.WriteLine("Enter Escape State"))
                    .Update(() =>
                    {
                        distanceBetweenOrc++;
                        distanceBetweenGoblin++;
                        Console.WriteLine($"Escape ING.... distanceBetweenOrc:{distanceBetweenOrc} distanceBetweenGobin:{distanceBetweenGoblin} Hp:{hp} orcHp:{orcHp}");
                    })
                    .Translate(() => distanceBetweenOrc > 5 * r).To(State.Patrol)
                .End()
                .NewState(State.SayHi)
                    .Initialize(() => Console.WriteLine("Init SayHi State"))
                    .Enter(() => Console.WriteLine("Enter SayHi State"))
                    .Update(() =>
                    {
                        sayHiComplete = true;
                        Console.WriteLine("Say Hi to Goblin");
                    })
                    .Translate(() => sayHiComplete).To(State.Patrol)
                .End()
                .Initialize()
                .Start(State.Patrol);

            // update machine
            bool running = true;
            ThreadPool.QueueUserWorkItem(_ => { var key = Console.ReadKey(); running = false; });
            while (running)
            {
                machine.Update();
                Thread.Sleep(100);
            }
            Console.WriteLine(Guid.NewGuid().ToString());
       
            machine.Stop();
            Console.WriteLine("FSM Stop");
        }
        
    }
}
