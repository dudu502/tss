using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Decorator
{
    public abstract class DecoratorNode : Node
    {
        protected Node Child;
        public void SetChild(Node node)
        {
            Child = node;
        }
    }
}
