using System;
using System.Collections.Generic;
using System.Text;

namespace Task.Switch.Structure.BT.Composites
{
    public abstract class CompositeNode : Node
    {
        protected readonly List<Node> m_Children = new List<Node>();

        public void AddChild(Node child)
        {
            m_Children.Add(child);
        }
    }
}
