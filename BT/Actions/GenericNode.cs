namespace Task.Switch.Structure.BT.Actions
{
    public class GenericNode : ActionNode
    {
        protected override NodeResult GetResult()
        {
            if (m_NodeResult == null)
                return NodeResult.Success;
            return m_NodeResult();
        }
    }
}
