namespace Task.Switch.Structure.BT.Decorators
{
    public class RepeatNode : DecoratorNode
    {
        protected override NodeResult GetResult()
        {
            m_Child.Execute();
            return NodeResult.Continue;
        }
    }
}
