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
        protected string m_Tag;
        public Node<T> Start(Action<T> start) { m_Start = start; return this; }
        public Node<T> Stop(Action<T> stop) { m_Stop = stop; return this; }
        public Node<T> Update(Action<T> update) { m_Update = update; return this; }
        public Node<T> SetResult(Func<T, NodeResult> result) { m_NodeResult = result; return this; }
        public NodeResult Execute()
        {
            if (!m_Started)
            {
                OnStart();

                if (BehaviourTreeDebug.Log != null && (BehaviourTreeDebug.Filter & BehaviourTreeDebug.LogFilter.OnStart) == BehaviourTreeDebug.LogFilter.OnStart)
                    BehaviourTreeDebug.Log($"Node:{GetType().Name} Tag:{m_Tag ?? "<null>"} OnStart Parameter:{Tree.Parameter}");

                m_Started = true;
            }
            OnUpdate();

            if (BehaviourTreeDebug.Log != null && (BehaviourTreeDebug.Filter & BehaviourTreeDebug.LogFilter.OnUpdate) == BehaviourTreeDebug.LogFilter.OnUpdate)
                BehaviourTreeDebug.Log($"Node:{GetType().Name} Tag:{m_Tag ?? "<null>"} OnUpdate Parameter:{Tree.Parameter}");

            Result = GetResult();

            if (BehaviourTreeDebug.Log != null && (BehaviourTreeDebug.Filter & BehaviourTreeDebug.LogFilter.OnResult) == BehaviourTreeDebug.LogFilter.OnResult)
                BehaviourTreeDebug.Log($"Node:{GetType().Name} Tag:{m_Tag ?? "<null>"} Result:{Result} Parameter:{Tree.Parameter}");

            if (Result == NodeResult.Success || Result == NodeResult.Failure)
            {
                OnStop();

                if (BehaviourTreeDebug.Log != null && (BehaviourTreeDebug.Filter & BehaviourTreeDebug.LogFilter.OnStop) == BehaviourTreeDebug.LogFilter.OnStop)
                    BehaviourTreeDebug.Log($"Node:{GetType().Name} Tag:{m_Tag ?? "<null>"} OnStop Parameter:{Tree.Parameter}");

                m_Started = false;
            }
            return Result;
        }

        public Node<T> SetTag(string tag)
        {
            m_Tag = tag;
            return this;
        }

        public string GetTag()
        {
            return m_Tag;
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
