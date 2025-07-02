
namespace Task.Switch.Structure.BT.Composites
{
    public class SequencerNode<T> : CompositeNode<T>
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
            if(m_Children.Count == 0)return NodeResult.Success;
            Node<T> child = m_Children[m_CurrentIndex];
            switch (child.Execute())
            {
                case NodeResult.Continue:
                    return NodeResult.Continue;
                case NodeResult.Failure:
                    return NodeResult.Failure;
                case NodeResult.Success:
                    m_CurrentIndex++;
                    break;
            }
            return m_CurrentIndex == m_Children.Count ? NodeResult.Success : NodeResult.Continue;
        }
    }
}
