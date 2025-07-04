using System;
using Task.Switch.Structure.BT.Decorators;

namespace Task.Switch.Structure.BT
{
    public abstract class Node<T>
    {
        public BehaviourTree<T> Tree;
        public NodeResult Result { private set; get; } = NodeResult.Continue;
        private bool m_Started = false;    
        private Action<T> m_Start;
        private Action<T> m_Stop;
        private Action<T> m_Update;
        protected Func<T,NodeResult> m_NodeResult;
        public Node<T> Start(Action<T> start) { m_Start = start; return this; }
        public Node<T> Stop(Action<T> stop) { m_Stop = stop; return this; }
        public Node<T> Update(Action<T> update) { m_Update = update; return this; }
        public Node<T> SetResult(Func<T, NodeResult> result) { m_NodeResult = result; return this; }
        public NodeResult Execute()
        {
            if (!m_Started)
            {
                OnStart();
                m_Started = true;
            }
            OnUpdate();
            Result = GetResult();
            if (Result == NodeResult.Success || Result == NodeResult.Failure)
            {
                OnStop();
                m_Started = false;
            }
            return Result;
        }

        public virtual void AddChild(Node<T> node)
        {

        }
        public virtual void Reset()
        {
            Result = NodeResult.Continue;
        }
        public BehaviourTree<T> End()
        {
            return Tree.End();         
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

    public sealed class RootNode<T> : DecoratorNode<T>
    {
        protected override NodeResult GetResult()
        {
            if (m_Child != null)
                return m_Child.Execute();
            return NodeResult.Success;
        }
    }
}
