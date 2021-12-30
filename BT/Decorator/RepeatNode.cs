using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Decorator
{
    public class RepeatNode : DecoratorNode
    {
        protected override void OnStart()
        {
            Console.WriteLine("Repeat Start");
        }

        protected override void OnStop()
        {
            Console.WriteLine("Repeat OnStop");
        }

        protected override NodeResult OnUpdate()
        {
            Console.WriteLine("Repeat OnUpdate");
            Child.Execute();
            return NodeResult.Continue;
        }
    }
}
