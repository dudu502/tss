using System;
namespace Task.Switch.Structure.BT.Decorators
{
    public class RepeatUntilNode:RepeatNode
    {
        private readonly Func<bool> m_RepeatUntil;
        private readonly NodeResult m_RepeatUntilResult;
        public RepeatUntilNode(Func<bool> repeatUtil,NodeResult nodeResult)
        {
            m_RepeatUntil = repeatUtil;
            m_RepeatUntilResult = nodeResult;
        }
        protected override NodeResult GetResult()
        {
            NodeResult result = base.GetResult();
            if(m_RepeatUntil!=null&&m_RepeatUntil.Invoke())
                return m_RepeatUntilResult;
            return result;               
        }
    }
}
