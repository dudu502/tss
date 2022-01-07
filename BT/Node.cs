
using System;

namespace Task.Switch.Structure.BT
{
    public abstract class Node
    {
        public enum NodeResult
        {
            Continue = 1,
            Failure = 2,
            Success = 4
        }
        public BehaviourTree Tree;
        public NodeResult Result { private set; get; } = NodeResult.Continue;
        private bool m_Started = false;
       
        protected Action m_Start;
        protected Action m_Stop;
        protected Action m_Update;
        protected Func<NodeResult> m_NodeResult;
        public Node Start(Action start) { m_Start = start; return this; }
        public Node Stop(Action stop) { m_Stop = stop; return this; }
        public Node Update(Action update) { m_Update = update; return this; }

        public Node GetResult(Func<NodeResult> result) { m_NodeResult = result; return this; }
        public NodeResult Execute()
        {
            if (!m_Started)
            {
                OnStart();
                m_Started = true;
            }
            OnUpdate();
            Result = GetResult();    
            if(Result == NodeResult.Success||Result == NodeResult.Failure)
            {
                OnStop();
                m_Started = false;
            }
            return Result;
        }
        public virtual void Reset()
        {
            Result = NodeResult.Continue;
        }
        public BehaviourTree.TreeBuilder End()
        {
            return Tree.Builder.End();         
        }
        protected virtual void OnStart() 
        {
            m_Start?.Invoke();
        }
        protected virtual void OnUpdate() 
        {
            m_Update?.Invoke();
        }
        protected abstract NodeResult GetResult();
        protected virtual void OnStop() 
        {
            m_Stop?.Invoke();
        }    
    }   

    public sealed class RootNode : Node
    {
        private Node m_Child;
        public void SetChild(Node node)
        {
            m_Child = node;
        }

        protected override NodeResult GetResult()
        {
            if(m_Child != null)
                return m_Child.Execute();
            return NodeResult.Success;
        }
        public override void Reset()
        {
            base.Reset();
            if (m_Child != null)
                m_Child.Reset();
        }
    }
}
