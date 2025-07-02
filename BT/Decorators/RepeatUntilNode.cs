using System;
namespace Task.Switch.Structure.BT.Decorators
{
    public class RepeatUntilNode<T> : RepeatNode<T>
    {
        private readonly Func<bool> m_RepeatUntil;
        public RepeatUntilNode(Func<bool> repeatUtil)
        {
            m_RepeatUntil = repeatUtil;
        }
        protected override NodeResult GetResult()
        {
            NodeResult result = base.GetResult();
            if (m_RepeatUntil != null && m_RepeatUntil.Invoke())
            {
                if (m_NodeResult != null)
                    return m_NodeResult.Invoke(Tree.Parameter);
                return NodeResult.Success;
            }
            return result;
        }
    }
}