namespace Task.Switch.Structure.BT.Composites
{
    public class SelectorNode:CompositeNode
    {
        int m_CurrentIndex;
        protected override void OnStart()
        {
            base.OnStart();
            m_CurrentIndex = 0;
        }
        public override void Reset()
        {
            base.Reset();
            m_CurrentIndex = 0;
        }
        protected override NodeResult GetResult()
        {
            if(m_Children.Count == 0)return NodeResult.Failure;
            Node child = m_Children[m_CurrentIndex];
            switch (child.Execute())
            {
                case NodeResult.Continue:
                    return NodeResult.Continue;
                case NodeResult.Success:
                    return NodeResult.Success;
                case NodeResult.Failure:
                    m_CurrentIndex++;
                    break;
            }
            return m_CurrentIndex == m_Children.Count ? NodeResult.Failure : NodeResult.Continue;
        }
    }
}
