using System;
using System.Threading;

namespace Task.Switch.Structure.FSM
{
    class Program
    {
        enum State
        {
            Idle,
            Run,
        }

        class StateObject
        {
            public const int MAX_PHYSICAL_STRENGTH = 50;
            public int physical_strength = 25;

            public void Log()
            {
                Console.WriteLine("Current physical_strength " + physical_strength);
            }
        }

        static void Main(string[] args)
        {
            // Create a state machine
            StateMachine<State, StateObject>.Log = Console.WriteLine;
            var machine = new StateMachine<State, StateObject>(new StateObject());
            machine
                .NewState(State.Idle)
                    .Initialize((stateObj) => { })
                    .Enter((stateObj) => { })
                    .Update((stateObj) =>
                    {
                        stateObj.physical_strength++;
                        stateObj.Log();
                    })
                    .Translate((stateObj) => stateObj.physical_strength >= StateObject.MAX_PHYSICAL_STRENGTH).To(State.Run)
                    .Exit((stateObj) => { })
                .End()
                .NewState(State.Run)
                    .Initialize((stateObj) => { })
                    .Enter((stateObj) => { })
                    .Update((stateObj) =>
                    {
                        stateObj.physical_strength--;
                        stateObj.Log();
                    })
                    .Translate((stateObj) => stateObj.physical_strength <= 0).To(State.Idle)
                    .Exit((stateObj) => { })
                .End()
                .Initialize().Start(State.Idle);

            bool running = true;
            ThreadPool.QueueUserWorkItem(_ => { var key = Console.ReadKey(); running = false; });
            while (running)
            {
                machine.Tick();
                Thread.Sleep(100);
            }

            machine.Stop();
            Console.WriteLine("FSM Stop");
        }
    }
}
