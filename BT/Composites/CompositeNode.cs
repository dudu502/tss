using System.Collections.Generic;

namespace Task.Switch.Structure.BT.Composites
{
    public abstract class CompositeNode<T> : Node<T>
    {
        protected readonly List<Node<T>> m_Children = new List<Node<T>>();
        public override void AddChild(Node<T> node)
        {
            m_Children.Add(node);
        }
        public override void Reset()
        {
            base.Reset();
            foreach (Node<T> node in m_Children)
                node.Reset();
        }
    }
}
