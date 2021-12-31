namespace Task.Switch.Structure.BT.Actions
{
    public class WaitFrameNode : ActionNode
    {
        readonly int m_WaitFrameCount;
        int m_FrameCount = 0;
        readonly NodeResult m_WaitResult;
        public WaitFrameNode(int frameCount, NodeResult waitResult)
        {
            m_WaitFrameCount = frameCount;
            m_WaitResult = waitResult;
        }
        protected override void OnStart()
        {
            base.OnStart();
            m_FrameCount = 0;
        }


        protected override NodeResult GetResult()
        {
            if (m_FrameCount++ >= m_WaitFrameCount)
                return m_WaitResult;
            return NodeResult.Continue;
        }
    }
}
