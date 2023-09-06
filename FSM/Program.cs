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
            public string name;
            public StateObject(string n,int value)
            {
                name = n;
                physical_strength = value;
            }
            public void Log()
            {
                Console.WriteLine(name + "Current physical_strength " + physical_strength);
            }
            public void Log(string value)
            {
                Console.WriteLine(value);
            }
        }

        static void Main(string[] args)
        {
            IStateMachine<StateObject> machine = new StateMachine<StateObject>(new StateObject("TOM ", 0))
                .State(State.Idle)
                    .Initialize(so => so.Log("Init Idle"))
                    .Enter(so => so.Log("Enter Idle"))
                    .Update(so => { so.physical_strength++; so.Log(); })
                    .Exit(so => so.Log("Exit Idle"))
                    .Transition(so => so.physical_strength >= StateObject.MAX_PHYSICAL_STRENGTH)
                        .Transfer(so => so.Log("Transfer Idle"))
                        .To(State.Run)
                    .End()
                .End()
                .State(State.Run)
                    .Initialize(so => so.Log("Init Run"))
                    .Enter(so => so.Log("Enter Run"))
                    .Update(so => { so.physical_strength--; so.Log(); })
                    .Exit(so => so.Log("Exit Run"))
                    .Transition(so => so.physical_strength <= 0)
                        .Transfer(so => so.Log("Transfer Run"))
                        .To(State.Idle)
                    .End()
                .End()
                .SetDefault(State.Idle)
                .Build();
            bool running = true;
            IStateMachine<StateObject> i = StateMachine<StateObject>.Clone(machine, new StateObject("Jeffy ", 47)).Build();
            ThreadPool.QueueUserWorkItem(_ => { var key = Console.ReadKey(); running = false; });
            while (running)
            {
                machine.Update();
                i.Update();
                Thread.Sleep(50);
            }

            Console.WriteLine("FSM Stop");
        }
    }
}
