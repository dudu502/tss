
using System;
using System.Collections.Generic;
using Task.Switch.Structure.BT.Actions;
using Task.Switch.Structure.BT.Composites;
using Task.Switch.Structure.BT.Decorators;

namespace Task.Switch.Structure.BT
{
    public enum NodeResult
    {
        Continue = 1,
        Failure = 2,
        Success = 4
    }

    public class BehaviourTreeDebug
    {
        public enum LogFilter
        {
            Nothing = 0,
            OnStart = 1,
            OnUpdate = 2,
            OnStop = 4,
            OnResult = 8,
            Everything = OnStart | OnUpdate | OnStop | OnResult,
        }
        public static Action<string> Log;
        public static LogFilter Filter = LogFilter.Nothing;
    }

    public class BehaviourTree<T>
    {
        private readonly RootNode<T> m_Root = new RootNode<T>();
        private readonly Stack<Node<T>> m_PointerStack = new Stack<Node<T>>();
        private NodeResult m_TreeState = NodeResult.Continue;
        public T Parameter { get; private set; }
        public BehaviourTree(T para)
        {
            Parameter = para;
            PushNodeToTree(m_Root);
        }

        public void Reset()
        {
            m_Root?.Reset();
        }

        public NodeResult Execute()
        {
            if (m_Root.Result == NodeResult.Continue)
                m_TreeState = m_Root.Execute();
            return m_TreeState;
        }

        public BehaviourTree<T> Repeat(string tag = null)
        {
            PushNodeToTree(new RepeatNode<T>().SetTag(tag));
            return this;
        }
        public BehaviourTree<T> Success(string tag = null)
        {
            PushNodeToTree(new ReturnSuccessNode<T>().SetTag(tag));
            return this;
        }
        public BehaviourTree<T> Failure(string tag = null)
        {
            PushNodeToTree(new ReturnFailureNode<T>().SetTag(tag));
            return this;
        }
        public BehaviourTree<T> Invert(string tag = null)
        {
            PushNodeToTree(new InverterNode<T>().SetTag(tag));
            return this;
        }
        public BehaviourTree<T> RepeatUntil(Func<T, bool> repeatUntil, string tag = null)
        {
            PushNodeToTree(new RepeatUntilNode<T>(repeatUntil).SetTag(tag));
            return this;
        }
        public BehaviourTree<T> Sequence(string tag = null)
        {
            PushNodeToTree(new SequencerNode<T>().SetTag(tag));
            return this;
        }
        public BehaviourTree<T> Select(string tag = null)
        {
            PushNodeToTree(new SelectorNode<T>().SetTag(tag));
            return this;
        }
        public BehaviourTree<T> Parallel(string tag = null)
        {
            PushNodeToTree(new ParallelNode<T>().SetTag(tag));
            return this;
        }
        public BehaviourTree<T> ParallelSelect(string tag = null)
        {
            PushNodeToTree(new ParallelSelectNode<T>().SetTag(tag));
            return this;
        }
        public BehaviourTree<T> ParallelSequence(string tag = null)
        {
            PushNodeToTree(new ParallelSequencerNode<T>().SetTag(tag));
            return this;
        }
        public Node<T> Do(string tag = null)
        {
            return PushNodeToTree(new GenericNode<T>().SetTag(tag));
        }
        public Node<T> WaitTime(int ms, string tag = null)
        {
            return PushNodeToTree(new WaitTimeNode<T>(ms).SetTag(tag));
        }
        public Node<T> WaitTurn(int frameCount, string tag = null)
        {
            return PushNodeToTree(new WaitTurnNode<T>(frameCount).SetTag(tag));
        }
        public Node<T> WaitUntil(Func<T, bool> waitFunc, string tag = null)
        {
            return PushNodeToTree(new WaitUntilNode<T>(waitFunc).SetTag(tag));
        }
        public BehaviourTree<T> End()
        {
            if (m_PointerStack.Count > 0)
                m_PointerStack.Pop();
            return this;
        }
        Node<T> PushNodeToTree(Node<T> child)
        {
            child.Tree = this;
            if (m_PointerStack.TryPeek(out Node<T> parent))
                parent.AddChild(child);
            m_PointerStack.Push(child);
            return child;
        }
    }
}