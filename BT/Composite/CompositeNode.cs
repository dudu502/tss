using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Composite
{
    public abstract class CompositeNode : Node
    {
        public List<Node> Children = new List<Node>();

    }

}
