
namespace Task.Switch.Structure.BT.Composites
{
    public class ParallelSelectNode : CompositeNode
    {
        protected override NodeResult GetResult()
        {
            NodeResult result = NodeResult.Failure;
            foreach (Node child in m_Children)
            {
                result = child.Execute();
                if (result == NodeResult.Failure) 
                    continue;
                else 
                    break;              
            }
            return result;
        }
    }
}
