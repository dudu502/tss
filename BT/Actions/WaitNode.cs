using System;

namespace Task.Switch.Structure.BT.Actions
{
    public class WaitNode : ActionNode
    {
        readonly TimeSpan m_WaitTimeSpan;
        DateTime m_StartDateTime;
        readonly NodeResult m_WaitResult;
        public WaitNode(int ms, NodeResult waitResult)
        {
            m_WaitTimeSpan = new TimeSpan(ms * 10000);
            m_WaitResult = waitResult;
        }

        protected override void OnStart()
        {
            base.OnStart();
            m_StartDateTime = DateTime.Now;
        }

        public override void Reset()
        {
            base.Reset();
            m_StartDateTime = DateTime.Now;
        }

        protected override NodeResult GetResult()
        {
            if (DateTime.Now - m_StartDateTime > m_WaitTimeSpan)
                return m_WaitResult;
            return NodeResult.Continue;
        }
    }
}