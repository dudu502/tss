
namespace Task.Switch.Structure.BT.Composites
{
    public class ParallelNode<T> : CompositeNode<T>
    {
        protected override NodeResult GetResult()
        {
            NodeResult state = NodeResult.Success;
            foreach (Node<T> child in m_Children)
                state |= child.Execute();
            if ((state & NodeResult.Failure) == NodeResult.Failure)
                return NodeResult.Failure;
            if ((state & NodeResult.Continue) == NodeResult.Continue)
                return NodeResult.Continue;
            return NodeResult.Success;
        }
    }
}