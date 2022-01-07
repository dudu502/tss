
namespace Task.Switch.Structure.BT.Decorators
{
    public class InverterNode : DecoratorNode
    {
        protected override NodeResult GetResult()
        {
            NodeResult result = NodeResult.Continue;
            switch(m_Child.Execute())
            {
                case NodeResult.Success:
                    result = NodeResult.Failure;
                    break;
                case NodeResult.Failure:
                    result = NodeResult.Success;
                    break;
            }
            return result;
        }
    }
}
