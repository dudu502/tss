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

            int timer1 = 0;
            const int maxTimer1Count = 20;
            // add new state
            machine.NewState("空闲")
                 .Initialize(() => Console.WriteLine("初始化 空闲"))
                 .Enter(() =>
                 {
                     timer = 0;
                     Console.WriteLine("进入 空闲");
                 })
                 .Update(() => Console.WriteLine("空闲... " + timer++))
                 .Exit(() => Console.WriteLine("退出 空闲"))
                 .Translate("空闲=>巡逻").Valid(() => timer >= maxTimerCount).Transfer(() => Console.WriteLine("转化中 ...")).To("巡逻");

            machine.NewState("巡逻")
                .Initialize(() => Console.WriteLine("初始化 巡逻"))
                .Enter(() =>
                {
                    timer = 0;
                    Console.WriteLine("进入 巡逻");
                })
                .Update(() => Console.WriteLine("巡逻... " + timer++ + " 危险系数" + timer1++))
                .Exit(() => Console.WriteLine("退出 巡逻"))
                .Translate("巡逻=>逃跑").Valid(() => timer1 > maxTimer1Count).To("逃跑")
                .Translate("巡逻=>空闲").Valid(() => timer >= maxTimerCount).Transfer(() => Console.WriteLine("转化中 ...")).To("空闲");

            machine.NewState("逃跑")
               .Initialize(() => Console.WriteLine("初始化 逃跑"))
               .Enter(() =>
               {
                   timer = 0;
                   Console.WriteLine("进入 逃跑");
               })
               .Update(() => Console.WriteLine("逃跑... " + timer1--))
               .Exit(() => Console.WriteLine("退出 逃跑"))
               .Translate("逃跑=>空闲").Valid(() => timer1 == 0).Transfer(() => Console.WriteLine("转化中 ...")).To("空闲");

            

            //initialize machine
            machine.Initialize();

            // start machine
            ThreadPool.QueueUserWorkItem(ob =>
            {
                machine.Start("空闲");
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
