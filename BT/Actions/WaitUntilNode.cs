using System;

namespace Task.Switch.Structure.BT.Actions
{
    public class WaitUntilNode<T> : ActionNode<T>
    {
        private readonly Func<T, bool> m_WaitUntilFunc;
        public WaitUntilNode(Func<T, bool> waitUntil)
        {
            m_WaitUntilFunc = waitUntil;
        }

        protected override NodeResult GetResult()
        {
            if (m_WaitUntilFunc != null && m_WaitUntilFunc.Invoke(Tree.Parameter))
            {
                if (m_NodeResult != null)
                    return m_NodeResult.Invoke(Tree.Parameter);
                return NodeResult.Success;
            }
            return NodeResult.Continue;
        }
    }
}