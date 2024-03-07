using System;
using System.Collections.Generic;
using System.Threading;

namespace Task.Switch.Structure.FSM
{
    class Program
    {
        enum State
        {
            Normal,
            Stun,
        }

        class StateObject
        {
            public Queue<EventArgs> EventArgs;
            public int hp = 100;
            public int position;
            public StateObject()
            {

            }

            public void Log()
            {
                Console.WriteLine(ToString());
            }
            public override string ToString()
            {
                return $"HP:{hp} Position:{position} ";
            }
            public void OnHit(int value)
            {
                hp -= value;
                EventArgs.Enqueue(new FSM.EventArgs("hit",value));
            }
            public void OnStun()
            {
                hp -= 10;
                EventArgs.Enqueue(new FSM.EventArgs("stun", null));
            }

            public void OnReset()
            {
                EventArgs.Enqueue(new FSM.EventArgs("reset", null));
            }
        }

        static void Main(string[] args)
        {
            IStateMachine<StateObject> machine = new StateMachine<StateObject>(new StateObject())
                .State(State.Normal)
                    .Update(so=> { so.position++; so.Log(); })
                    .Transition(so => { 
                        while(so.EventArgs.TryDequeue(out EventArgs e))
                            if (e.EventType == "stun")
                                return true;
                        return false;
                    }).To(State.Stun).End()
                .End()
                .State(State.Stun)
                    .Update(so=>so.Log())
                    .Transition(so =>
                    {
                        while(so.EventArgs.TryDequeue(out EventArgs e))
                            if (e.EventType == "reset")
                                return true;
                        return false;
                    }).To(State.Normal).End()
                .End()
                .SetDefault(State.Normal)
                .Build();
            machine.GetParameter().EventArgs = machine.GetEventArgs();

            bool running = true;
            
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    var key = Console.ReadLine();
                    if (key == "d")
                    {
                        machine.GetParameter().OnHit(1);
                    }
                    else if (key == "s")
                    {
                        machine.GetParameter().OnStun();
                    }
                    else if(key == "n")
                    {
                        machine.GetParameter().OnReset();
                    }
                }
            });
            while (running)
            {
                machine.Update();

                Thread.Sleep(50);
            }
            
            Console.WriteLine("FSM Stop");
        }
    }
}
