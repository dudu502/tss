using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Action
{
    public class DebugNode : ActionNode
    {
        public string message;
        public NodeResult State = NodeResult.Success;
        protected override void OnStart()
        {
            Console.WriteLine("OnStart      " + message);
        }

        protected override void OnStop()
        {
            Console.WriteLine("OnStop       " + message);
        }

        protected override NodeResult OnUpdate()
        {
            Console.WriteLine("OnUpdate     " + message);
            //return State.Success;
            return State;
        }
    }
}
