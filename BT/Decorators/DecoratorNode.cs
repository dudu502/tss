namespace Task.Switch.Structure.BT.Decorators
{
    public abstract class DecoratorNode<T> : Node<T>
    {
        protected Node<T> m_Child;
        public override void AddChild(Node<T> node)
        {
            m_Child = node;
        }
        public override void Reset()
        {
            base.Reset();
            m_Child?.Reset();
        }
    }
}
