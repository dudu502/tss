using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Actions
{
    public abstract class ActionNode : Node
    {
        protected System.Action m_Start;
        protected System.Action m_Stop;
        protected System.Func<NodeResult> m_Update;

        public ActionNode Start(System.Action start) { m_Start = start; return this; }
        public ActionNode Stop(System.Action stop) { m_Stop = stop; return this; }
        public ActionNode Update(System.Func<NodeResult> update) { m_Update = update; return this; }

        public BehaviourTree End()
        {
            return Tree;
        }
    }
}
