using System.Collections.Generic;

namespace Task.Switch.Structure.BT.Composites
{
    public abstract class CompositeNode<T> : Node<T>
    {
        protected readonly List<Node<T>> m_Children = new List<Node<T>>();
        public void AddChild(Node<T> child)
        {
            m_Children.Add(child);
        }
        public override void Reset()
        {
            base.Reset();
            foreach (Node<T> node in m_Children)
                node.Reset();
        }
    }
}
