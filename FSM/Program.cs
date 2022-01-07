using System;
using System.Threading;

namespace Task.Switch.Structure.FSM
{
    class Program
    {
        enum State
        {
            Idle,
            Patrol,
            Escape
        }
        static void Main(string[] args)
        {     
            // Create a state machine
            var machine = new StateMachine<State>();
            int timer = 0;
            const int maxTimerCount = 10;

            int timer1 = 0;
            const int maxTimer1Count = 20;
            // add new state
            machine
                .NewState(State.Idle)
                    .Initialize(() => Console.WriteLine("初始化 空闲"))
                    .Enter(() =>
                    {
                        timer = 0;
                        Console.WriteLine("进入 空闲");
                    })
                    .Update(() => Console.WriteLine("空闲... " + timer++))
                    .Exit(() => Console.WriteLine("退出 空闲"))
                    .Translate(() => timer >= maxTimerCount).Transfer(() => Console.WriteLine("转化中 ...")).To(State.Patrol)
                .End()
                    .NewState(State.Patrol)
                    .Initialize(() => Console.WriteLine("初始化 巡逻"))
                    .Enter(() =>
                    {
                        timer = 0;
                        Console.WriteLine("进入 巡逻");
                    })
                    .Update(() => Console.WriteLine("巡逻... " + timer++ + " 危险系数" + timer1++))
                    .Exit(() => Console.WriteLine("退出 巡逻"))
                    .Translate(() => timer1 > maxTimer1Count).To(State.Escape)
                    .Translate(() => timer >= maxTimerCount).Transfer(() => Console.WriteLine("转化中 ...")).To(State.Idle)
                .End().NewState(State.Escape)
                    .Initialize(() => Console.WriteLine("初始化 逃跑"))
                    .Enter(() =>
                    {
                        timer = 0;
                        Console.WriteLine("进入 逃跑");
                    })
                    .Update(() => Console.WriteLine("逃跑... " + timer1--))
                    .Exit(() => Console.WriteLine("退出 逃跑"))
                    .Translate(() => timer1 == 0).Transfer(() => Console.WriteLine("转化中 ...")).To(State.Idle)
                .End()
                .Initialize()
                .Start(State.Idle);

            

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
