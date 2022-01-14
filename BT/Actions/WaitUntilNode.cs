using System;

namespace Task.Switch.Structure.BT.Actions
{
    public class WaitUntilNode : ActionNode
    {
        private readonly Func<bool> m_WaitUntilFunc;
        public WaitUntilNode(Func<bool> waitUntil)
        {
            m_WaitUntilFunc = waitUntil;
        }

        protected override NodeResult GetResult()
        {
            if (m_WaitUntilFunc != null && m_WaitUntilFunc.Invoke())
            {
                if (m_NodeResult != null)
                    return m_NodeResult.Invoke();
                return NodeResult.Success;
            }
            return NodeResult.Continue;
        }
    }
}