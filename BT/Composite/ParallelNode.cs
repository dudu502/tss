using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Composite
{
    public class ParallelNode: CompositeNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override NodeResult OnUpdate()
        {
            NodeResult state = NodeResult.Success;
            foreach (Node child in Children)
                state |= child.Execute();
            if ((state & NodeResult.Failure) == NodeResult.Failure)
                return NodeResult.Failure;
            else if ((state & NodeResult.Continue) == NodeResult.Continue)
                return NodeResult.Continue;
            return NodeResult.Success;
        }


    }
}
