using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Composite
{
    public abstract class CompositeNode : Node
    {
        protected List<Node> Children = new List<Node>();

        public void AddChild(Node child)
        {
            Children.Add(child);
        }
    }

}
