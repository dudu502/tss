using System;

namespace Task.Switch.Structure.BT.Actions
{
    public class WaitTimeNode<T> : ActionNode<T>
    {
        readonly TimeSpan m_WaitTimeSpan;
        DateTime m_StartDateTime;

        public WaitTimeNode(int ms)
        {
            m_WaitTimeSpan = new TimeSpan(ms * 10000);
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
            {
                if (m_NodeResult != null)
                    return m_NodeResult.Invoke(Tree.Parameter);
                return NodeResult.Success;
            }
            return NodeResult.Continue;
        }
    }
}