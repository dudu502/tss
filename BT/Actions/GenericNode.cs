namespace Task.Switch.Structure.BT.Actions
{
    public class GenericNode<T> : ActionNode<T>
    {
        protected override NodeResult GetResult()
        {
            if (m_NodeResult == null)
                return NodeResult.Success;
            return m_NodeResult.Invoke(Tree.Parameter);
        }
    }
}
