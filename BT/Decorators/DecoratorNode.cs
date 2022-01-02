namespace Task.Switch.Structure.BT.Decorators
{
    public abstract class DecoratorNode : Node
    {
        protected Node m_Child;
        public void SetChild(Node node)
        {
            m_Child = node;
        }
        public override void Reset()
        {
            base.Reset();
            if (m_Child != null)
                m_Child.Reset();
        }
    }
}
