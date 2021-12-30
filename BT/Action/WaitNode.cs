using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Action
{
    public class WaitNode : ActionNode
    {
        public TimeSpan Ts = new TimeSpan(10000000);
        DateTime startDateTime;
        protected override void OnStart()
        {
            startDateTime = DateTime.Now;
        }

        protected override void OnStop()
        {

        }

        protected override NodeResult OnUpdate()
        {
            if (DateTime.Now - startDateTime > Ts)
                return NodeResult.Success;
            return NodeResult.Continue;
        }
    }
}
