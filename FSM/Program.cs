using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace Task.Switch.Structure.FSM
{
    public class Timer
    {
        public DateTime startTime;
        public TimeSpan Elapsed => DateTime.Now - startTime;

        public Timer()
        {
            startTime = DateTime.Now;
        }

        public void Reset()
        {
            startTime = DateTime.Now;
        }

        public static bool operator >(Timer timer, float duration)
            => timer.Elapsed.Seconds > duration;

        public static bool operator <(Timer timer, float duration)
            => timer.Elapsed.Seconds < duration;

        public static bool operator >=(Timer timer, float duration)
            => timer.Elapsed.Seconds >= duration;

        public static bool operator <=(Timer timer, float duration)
            => timer.Elapsed.Seconds <= duration;

        public static float operator /(Timer timer, float duration)
            => timer.Elapsed.Seconds / duration;
    }
    class Program
    {
        enum State
        {
            Normal,
            Stun,
        }

        class StateObject
        {
            public List<EventArgs> EventArgs;
            public int hp = 100;
            public int position;
            public Timer timer;
            public StateObject()
            {
                timer = new Timer();
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
                EventArgs.Add(new FSM.EventArgs("hit",value));
            }
            public void OnStun()
            {
                hp -= 10;
                EventArgs.Add(new FSM.EventArgs("stun", null));
            }

            public void OnReset()
            {
                EventArgs.Add(new FSM.EventArgs("reset", null));
            }
        }

        static void Main(string[] args)
        {
            IStateMachine<StateObject> machine = new StateMachine<StateObject>(new StateObject())
                .State(State.Normal)
                    .Update(so=> 
                    {   
                        so.position++; so.Log();
                    })
                    .Transition(so => 
                    { 
                        foreach(var evt in so.EventArgs)
                            if (evt.EventType == "stun")
                                return true;
                        return false;
                    }).To(State.Stun).End()
                .End()
                .State(State.Stun)
                    .Enter(so=>so.timer.Reset())
                    .Update(so=>so.Log())
                    .Transition(so =>
                    {
                        foreach (var evt in so.EventArgs)
                            if (evt.EventType == "reset")
                                return true;
                        return false;
                    }).To(State.Normal).End()
                    .Transition(so=>so.timer>2).Return().End()
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
