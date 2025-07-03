
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

        public BehaviourTree<T> Repeat()
        {
            PushNodeToTree(new RepeatNode<T>());
            return this;
        }
        public BehaviourTree<T> Success()
        {
            PushNodeToTree(new ReturnSuccessNode<T>());
            return this;
        }
        public BehaviourTree<T> Failure()
        {
            PushNodeToTree(new ReturnFailureNode<T>());
            return this;
        }
        public BehaviourTree<T> Invert()
        {
            PushNodeToTree(new InverterNode<T>());
            return this;
        }
        public BehaviourTree<T> RepeatUntil(Func<T, bool> repeatUntil)
        {
            PushNodeToTree(new RepeatUntilNode<T>(repeatUntil));
            return this;
        }
        public BehaviourTree<T> Sequence()
        {
            PushNodeToTree(new SequencerNode<T>());
            return this;
        }
        public BehaviourTree<T> Select()
        {
            PushNodeToTree(new SelectorNode<T>());
            return this;
        }
        public BehaviourTree<T> Parallel()
        {
            PushNodeToTree(new ParallelNode<T>());
            return this;
        }
        public BehaviourTree<T> ParallelSelect()
        {
            PushNodeToTree(new ParallelSelectNode<T>());
            return this;
        }
        public BehaviourTree<T> ParallelSequence()
        {
            PushNodeToTree(new ParallelSequencerNode<T>());
            return this;
        }
        public Node<T> Do()
        {
            GenericNode<T> genericNode = new GenericNode<T>();
            PushNodeToTree(genericNode);
            return genericNode;
        }
        public Node<T> WaitTime(int ms)
        {
            WaitTimeNode<T> waitNode = new WaitTimeNode<T>(ms);
            PushNodeToTree(waitNode);
            return waitNode;
        }
        public Node<T> WaitTurn(int frameCount)
        {
            WaitTurnNode<T> waitFrameNode = new WaitTurnNode<T>(frameCount);
            PushNodeToTree(waitFrameNode);
            return waitFrameNode;
        }
        public Node<T> WaitUntil(Func<T, bool> waitFunc)
        {
            WaitUntilNode<T> waitUntilNode = new WaitUntilNode<T>(waitFunc);
            PushNodeToTree(waitUntilNode);
            return waitUntilNode;
        }
        public BehaviourTree<T> End()
        {
            if (m_PointerStack.Count > 0)
                m_PointerStack.Pop();
            return this;
        }
        void PushNodeToTree(Node<T> child)
        {
            child.Tree = this;
            if (m_PointerStack.TryPeek(out Node<T> parent))
                parent.AddChild(child);
            m_PointerStack.Push(child);
        }
    }
}
