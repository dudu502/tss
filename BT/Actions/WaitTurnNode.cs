namespace Task.Switch.Structure.BT.Actions
{
    public class WaitTurnNode<T> : ActionNode<T>
    {
        readonly int m_WaitTurnCount;
        int m_TurnCount = 0;

        public WaitTurnNode(int turnCount)
        {
            m_WaitTurnCount = turnCount;
        }
        protected override void OnStart()
        {
            base.OnStart();
            m_TurnCount = 0;
        }
        public override void Reset()
        {
            base.Reset();
            m_TurnCount=0;
        }

        protected override NodeResult GetResult()
        {
            m_TurnCount++;
            if (m_TurnCount >= m_WaitTurnCount)
            {
                if (m_NodeResult != null)
                    return m_NodeResult.Invoke(Tree.Parameter);
                return NodeResult.Success;
            }
            return NodeResult.Continue;
        }
    }
}
