using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Actions
{
    public class WaitNode : ActionNode
    {
        readonly TimeSpan m_WaitTimeSpan;
        DateTime m_StartDateTime;
        NodeResult m_WaitResult;
        public WaitNode(int ms, NodeResult waitResult)
        {
            m_WaitTimeSpan = new TimeSpan(ms * 10000);
            m_WaitResult = waitResult;
        }
        protected override void OnStart()
        {
            m_StartDateTime = DateTime.Now;
        }

        protected override void OnStop()
        {

        }

        protected override NodeResult OnUpdate()
        {
            if (DateTime.Now - m_StartDateTime > m_WaitTimeSpan)
                return m_WaitResult;
            return NodeResult.Continue;
        }
    }
}
