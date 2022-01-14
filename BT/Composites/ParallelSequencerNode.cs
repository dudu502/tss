
namespace Task.Switch.Structure.BT.Composites
{
    public class ParallelSequencerNode : CompositeNode
    {
        protected override NodeResult GetResult()
        {
            NodeResult result = NodeResult.Success;
            foreach(Node child in m_Children)
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
