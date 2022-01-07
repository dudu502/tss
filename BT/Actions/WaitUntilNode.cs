using System;
using System.Collections.Generic;
using System.Text;

namespace Task.Switch.Structure.BT.Actions
{
    public class WaitUntilNode : ActionNode
    {
        private readonly Func<bool> m_WaitUntilFunc;
        private readonly NodeResult m_WaitResult;
        public WaitUntilNode(Func<bool> waitUntil, NodeResult waitResult)
        {
            m_WaitUntilFunc = waitUntil;
            m_WaitResult = waitResult;
        }

        protected override NodeResult GetResult()
        {
            if (m_WaitUntilFunc())
                return m_WaitResult;
            return NodeResult.Continue;
        }
    }
}