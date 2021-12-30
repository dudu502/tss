using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Composite
{
    public class SelectorNode:CompositeNode
    {
        int current;
        protected override void OnStart()
        {
            current = 0;
        }

        protected override void OnStop()
        {

        }

        protected override NodeResult OnUpdate()
        {
            var child = Children[current];
            switch (child.Execute())
            {
                case NodeResult.Continue:
                    return NodeResult.Continue;
                case NodeResult.Success:
                    return NodeResult.Success;
                case NodeResult.Failure:
                    current++;
                    break;
            }
            return current == Children.Count ? NodeResult.Failure : NodeResult.Continue;
        }
    }
}
