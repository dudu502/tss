
using System;
using System.Collections.Generic;
using Task.Switch.Structure.BT.Actions;
using Task.Switch.Structure.BT.Composites;
using Task.Switch.Structure.BT.Decorators;

namespace Task.Switch.Structure.BT
{
    public enum NodeResult : byte
    {
        Continue = 1,
        Failure = 2,
        Success = 4
    }
    public class BehaviourTree<T>
    {
        #region Builder
        public class TreeBuilder<T>
        {
            private readonly Stack<Node<T>> m_PointerStack = new Stack<Node<T>>();
            private readonly BehaviourTree<T> m_Tree;
            private readonly RootNode<T> m_Root;
            public TreeBuilder(BehaviourTree<T> tree, RootNode<T> root)
            {
                m_Tree = tree;
                m_Root = root;
                PushNodeToTree(m_Root);
            }
            public TreeBuilder<T> Repeat()
            {
                PushNodeToTree(new RepeatNode<T>());
                return this;
            }
            public TreeBuilder<T> Success()
            {
                PushNodeToTree(new ReturnFailureNode<T>());
                return this;
            }
            public TreeBuilder<T> Failure()
            {
                PushNodeToTree(new ReturnFailureNode<T>());
                return this;
            }
            public TreeBuilder<T> Invert()
            {
                PushNodeToTree(new InverterNode<T>());
                return this;
            }
            public TreeBuilder<T> RepeatUntil(Func<bool> repeatUntil)
            {
                PushNodeToTree(new RepeatUntilNode<T>(repeatUntil));
                return this;
            }
            public TreeBuilder<T> Sequence()
            {
                PushNodeToTree(new SequencerNode<T>());
                return this;
            }
            public TreeBuilder<T> Select()
            {
                PushNodeToTree(new SelectorNode<T>());
                return this;
            }
            public TreeBuilder<T> Parallel()
            {
                PushNodeToTree(new ParallelNode<T>());
                return this;
            }
            public TreeBuilder<T> ParallelSelect()
            {
                PushNodeToTree(new ParallelSelectNode<T>());
                return this;
            }
            public TreeBuilder<T> ParallelSequence()
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
            public Node<T> WaitUntil(Func<bool> waitFunc)
            {
                WaitUntilNode<T> waitUntilNode = new WaitUntilNode<T>(waitFunc);
                PushNodeToTree(waitUntilNode);
                return waitUntilNode;
            }
            public TreeBuilder<T> End()
            {
                if (m_PointerStack.Count > 0)
                    m_PointerStack.Pop();
                return this;
            }
            void PushNodeToTree(Node<T> child)
            {
                child.Tree = m_Tree;
                if(m_PointerStack.TryPeek(out Node<T> parent))
                {
                    switch (parent)
                    {
                        case ActionNode<T>:
                            return;
                        case RootNode<T>:
                            ((RootNode<T>)parent).SetChild(child);
                            break;
                        case DecoratorNode<T>:
                            ((DecoratorNode<T>)parent).SetChild(child);
                            break;
                        case CompositeNode<T>:
                            ((CompositeNode<T>)parent).AddChild(child);
                            break;
                    }
                }
                m_PointerStack.Push(child);
            }
        }
        #endregion

        public readonly TreeBuilder<T> Builder;
        private readonly RootNode<T> m_Root = new RootNode<T>();
        private NodeResult m_TreeState = NodeResult.Continue;
        public T Parameter { get; private set; }
        public BehaviourTree(T para)
        {
            Parameter = para;
            Builder = new TreeBuilder<T>(this, m_Root);
        }
  
        public void Reset()
        {
            if (m_Root != null)
            {
                m_Root.Reset();
            }
        }

        public NodeResult Execute()
        {
            if (m_Root.Result == NodeResult.Continue)
                m_TreeState = m_Root.Execute();
            return m_TreeState;
        }
    }
}
