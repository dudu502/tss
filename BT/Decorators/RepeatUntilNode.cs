using System;
namespace Task.Switch.Structure.BT.Decorators
{
    public class RepeatUntilNode<T> : RepeatNode<T>
    {
        private readonly Func<T, bool> m_RepeatUntil;
        public RepeatUntilNode(Func<T, bool> repeatUtil)
        {
            m_RepeatUntil = repeatUtil;
        }
        protected override NodeResult GetResult()
        {
            NodeResult result = base.GetResult();
            if (m_RepeatUntil != null && m_RepeatUntil.Invoke(Tree.Parameter))
            {
                if (m_NodeResult != null)
                    return m_NodeResult.Invoke(Tree.Parameter);
                return NodeResult.Success;
            }
            return result;
        }
    }
}