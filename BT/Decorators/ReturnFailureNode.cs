
namespace Task.Switch.Structure.BT.Decorators
{
    public class ReturnFailureNode<T> : DecoratorNode<T>
    {
        protected override NodeResult GetResult()
        {
            m_Child.Execute();
            return NodeResult.Failure;
        }
    }
}
