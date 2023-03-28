﻿using System;
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
                Console.WriteLine("Current physical_strength "+ physical_strength);
            }
        }

        static void Main(string[] args)
        {
            // Create a state machine
            StateMachine<State>.Log = Console.WriteLine;
            var machine = new StateMachine<State>(new StateObject());
            machine
                .NewState(State.Idle)
                    .Initialize(() => { })
                    .Enter(() => { })
                    .Update(() =>
                    {
                        machine.GetParameter<StateObject>().physical_strength++;
                        machine.GetParameter<StateObject>().Log();
                    })
                    .Exit(() => { })
                    .Translate(() => machine.GetParameter<StateObject>().physical_strength >= StateObject.MAX_PHYSICAL_STRENGTH).To(State.Run)
                .End()
                .NewState(State.Run)
                    .Initialize(() => { })
                    .Enter(() => { })
                    .Update(() =>
                    {
                        machine.GetParameter<StateObject>().physical_strength--;
                        machine.GetParameter<StateObject>().Log();
                    })
                    .Exit(() => { })
                    .Translate(() => machine.GetParameter<StateObject>().physical_strength <= 0).To(State.Idle)
                .End()
                .Initialize().Start(State.Idle);

            bool running = true;
            ThreadPool.QueueUserWorkItem(_ => { var key = Console.ReadKey(); running = false; });
            while (running)
            {
                machine.Update();
                Thread.Sleep(100);
            }
            
            machine.Stop();
            Console.WriteLine("FSM Stop");
        }   
    }
}
