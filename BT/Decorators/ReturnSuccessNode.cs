
namespace Task.Switch.Structure.BT.Decorators
{
    public class ReturnSuccessNode : DecoratorNode
    {
        protected override NodeResult GetResult()
        {
            m_Child.Execute();
            return NodeResult.Success;
        }
    }
}
