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
            public void OnAdd()
            {
                hp++;
            }
            public void OnHit(int value)
            {
                hp -= value;
                EventArgs.Add(new EventArgs("hit",value));
            }
            public void OnStun()
            {
                hp -= 10;
                EventArgs.Add(new EventArgs("stun", null));
            }

            public void OnReset()
            {
                EventArgs.Add(new EventArgs("reset", null));
            }
            public bool PollEvent(string name)
            {
                foreach(var evt in EventArgs)
                {
                    if (evt.EventType == name)
                        return true;
                }
                return false;
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
                    .Transition(so => EventArgs.PollEvent(so.EventArgs,"stun")).To(State.Stun).End()
                .End()
                .State(State.Stun)
                    .Enter(so=>so.timer.Reset())
                    .Update(so=>so.Log())
                    .Transition(so=> EventArgs.PollEvent(so.EventArgs,"reset")).To(State.Normal).End()
                    .Transition(so=>so.timer>5).Return().End()
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
                    else if(key == "a")
                    {
                        machine.GetParameter().OnAdd();
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
