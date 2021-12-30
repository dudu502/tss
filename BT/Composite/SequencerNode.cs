using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Composite
{
    public class SequencerNode:CompositeNode
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
                case NodeResult.Failure:
                    return NodeResult.Failure;
                case NodeResult.Success:
                    current++;
                    break;
            }
            return current == Children.Count ? NodeResult.Success : NodeResult.Continue;

        }
    }
}
