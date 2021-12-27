using System;
using System.Threading;

namespace FSM
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a state machine
            var machine = new StateMachine();
            int timer = 0;
            const int maxTimerCount = 10;
            // add new state
            machine.NewState("Idle")
                 .Initialize(() => Console.WriteLine("Init Idle"))
                 .Enter(() =>
                 {
                     timer = 0;
                     Console.WriteLine("Enter Idle");
                 })
                 .Update(() => Console.WriteLine("Idle " + timer++))
                 .Exit(() => Console.WriteLine("Exit Idle"))
                 .Translate("Idle=>Run").Valid(() => timer >= maxTimerCount).Transfer(() => Console.WriteLine("Transfer ...")).To("Run");

            machine.NewState("Run")
                .Initialize(() => Console.WriteLine("Init Run"))
                .Enter(() =>
                {
                    timer = 0;
                    Console.WriteLine("Enter Run");
                })
                .Update(() => Console.WriteLine("Run " + timer++))
                .Exit(() => Console.WriteLine("Exit Run"))
                .Translate("Run=>Idle").Valid(() => timer >= maxTimerCount).Transfer(() => Console.WriteLine("Transfer ...")).To("Idle");

            //initialize machine
            machine.Initialize();

            // start machine
            ThreadPool.QueueUserWorkItem(ob =>
            {
                machine.Start("Idle");
                while (true)
                {
                    machine.Update();
                    Thread.Sleep(100);
                }

            }, null);

            // stop machine
            Console.ReadKey();
            machine.Stop();
        }
        
    }
}
