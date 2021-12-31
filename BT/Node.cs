using BT.Composite;
using BT.Decorator;
using System;
using System.Collections.Generic;
using System.Text;

namespace BT
{
    public abstract class Node
    {
        public enum NodeResult
        {
            Continue = 1,
            Failure = 2,
            Success = 4
        }
        public NodeResult Result { private set; get; } = NodeResult.Continue;
        private bool m_Started = false;
        public NodeResult Execute()
        {
            if (!m_Started)
            {
                OnStart();
                m_Started = true;
            }
            Result = OnUpdate();    
            if(Result == NodeResult.Success||Result == NodeResult.Failure)
            {
                OnStop();
                m_Started = false;
            }
            return Result;
        }
      
        protected abstract void OnStart();
        protected abstract NodeResult OnUpdate();
        protected abstract void OnStop();
    }   

    public class RootNode : Node
    {
        public Node Child;

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override NodeResult OnUpdate()
        {
            return Child.Execute();
        }


    }
}
