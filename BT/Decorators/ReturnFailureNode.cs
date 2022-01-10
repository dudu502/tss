
namespace Task.Switch.Structure.BT.Decorators
{
    public class ReturnFailureNode : DecoratorNode
    {
        protected override NodeResult GetResult()
        {
            m_Child.Execute();
            return NodeResult.Failure;
        }
    }
}
