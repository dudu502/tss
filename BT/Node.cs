using System;

namespace Task.Switch.Structure.BT
{
    public abstract class Node<T>
    {
        public BehaviourTree<T> Tree;
        public NodeResult Result { private set; get; } = NodeResult.Continue;
        private bool m_Started = false;    
        protected Action<T> m_Start;
        protected Action<T> m_Stop;
        protected Action<T> m_Update;
        protected Func<T,NodeResult> m_NodeResult;
        public Node<T> Start(Action<T> start) { m_Start = start; return this; }
        public Node<T> Stop(Action<T> stop) { m_Stop = stop; return this; }
        public Node<T> Update(Action<T> update) { m_Update = update; return this; }
        public Node<T> GetResult(Func<T, NodeResult> result) { m_NodeResult = result; return this; }
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
        public BehaviourTree<T>.TreeBuilder<T> End()
        {
            return Tree.Builder.End();         
        }
        protected virtual void OnStart() 
        {
            m_Start?.Invoke(Tree.Parameter);
        }
        protected virtual void OnUpdate() 
        {
            m_Update?.Invoke(Tree.Parameter);
        }
        protected abstract NodeResult GetResult();
        protected virtual void OnStop() 
        {
            m_Stop?.Invoke(Tree.Parameter);
        }    
    }   

    public sealed class RootNode<T> : Node<T>
    {
        private Node<T> m_Child;
        public void SetChild(Node<T> node)
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
