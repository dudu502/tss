namespace Task.Switch.Structure.BT.Actions
{
    public class WaitTickNode : ActionNode
    {
        readonly int m_WaitTurnCount;
        int m_TickCount = 0;
        readonly NodeResult m_WaitResult;
        public WaitTickNode(int turnCount, NodeResult waitResult)
        {
            m_WaitTurnCount = turnCount;
            m_WaitResult = waitResult;
        }
        protected override void OnStart()
        {
            base.OnStart();
            m_TickCount = 0;
        }
        public override void Reset()
        {
            base.Reset();
            m_TickCount=0;
        }

        protected override NodeResult GetResult()
        {
            m_TickCount++;
            if (m_TickCount >= m_WaitTurnCount)
                return m_WaitResult;
            return NodeResult.Continue;
        }
    }
}
