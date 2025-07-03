
namespace Task.Switch.Structure.BT.Composites
{
    public class ParallelSequencerNode<T> : CompositeNode<T>
    {
        protected override NodeResult GetResult()
        {
            NodeResult result = NodeResult.Success;
            foreach (Node<T> child in m_Children)
            {
                result = child.Execute();
                if (result == NodeResult.Success)
                    continue;
                else
                    break;
            }
            return result;
        }
    }
}
