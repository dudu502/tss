
using System;
using System.Collections.Generic;
using Task.Switch.Structure.BT.Actions;
using Task.Switch.Structure.BT.Composites;
using Task.Switch.Structure.BT.Decorators;

namespace Task.Switch.Structure.BT
{
    public class BehaviourTree
    {
        #region Builder
        public class TreeBuilder
        {
            private readonly Stack<Node> m_PointerStack = new Stack<Node>();
            private readonly BehaviourTree m_Tree;
            private readonly RootNode m_Root;
            public TreeBuilder(BehaviourTree tree, RootNode root)
            {
                m_Tree = tree;
                m_Root = root;
                PushNodeToTree(m_Root);
            }
            public TreeBuilder Repeat()
            {
                PushNodeToTree(new RepeatNode());
                return this;
            }
            public TreeBuilder Invert()
            {
                PushNodeToTree(new InverterNode());
                return this;
            }
            public TreeBuilder RepeatUntil(Func<bool> repeatUntil, Node.NodeResult repeatResult = Node.NodeResult.Success)
            {
                PushNodeToTree(new RepeatUntilNode(repeatUntil, repeatResult));
                return this;
            }
            public TreeBuilder Sequence()
            {
                PushNodeToTree(new SequencerNode());
                return this;
            }
            public TreeBuilder Select()
            {
                PushNodeToTree(new SelectorNode());
                return this;
            }
            public TreeBuilder Parallel()
            {
                PushNodeToTree(new ParallelNode());
                return this;
            }
            public Node Do()
            {
                GenericNode genericNode = new GenericNode();
                PushNodeToTree(genericNode);
                return genericNode;
            }
            public Node WaitTime(int ms, Node.NodeResult waitResult = Node.NodeResult.Success)
            {
                WaitTimeNode waitNode = new WaitTimeNode(ms, waitResult);
                PushNodeToTree(waitNode);
                return waitNode;
            }
            public Node WaitTick(int frameCount, Node.NodeResult waitResult = Node.NodeResult.Success)
            {
                WaitTickNode waitFrameNode = new WaitTickNode(frameCount, waitResult);
                PushNodeToTree(waitFrameNode);
                return waitFrameNode;
            }
            public Node WaitUntil(Func<bool> waitFunc, Node.NodeResult nodeResult = Node.NodeResult.Success)
            {
                WaitUntilNode waitUntilNode = new WaitUntilNode(waitFunc, nodeResult);
                PushNodeToTree(waitUntilNode);
                return waitUntilNode;
            }
            public TreeBuilder End()
            {
                if (m_PointerStack.Count > 0)
                    m_PointerStack.Pop();
                return this;
            }
            void PushNodeToTree(Node child)
            {
                child.Tree = m_Tree;
                if(m_PointerStack.TryPeek(out Node parent))
                {
                    switch (parent)
                    {
                        case ActionNode:
                            return;
                        case RootNode:
                            ((RootNode)parent).SetChild(child);
                            break;
                        case DecoratorNode:
                            ((DecoratorNode)parent).SetChild(child);
                            break;
                        case CompositeNode:
                            ((CompositeNode)parent).AddChild(child);
                            break;
                    }
                }
                m_PointerStack.Push(child);
            }
        }
        #endregion
        private readonly RootNode m_Root = new RootNode();
        private Node.NodeResult m_TreeState = Node.NodeResult.Continue;
        public readonly TreeBuilder Builder;
        public BehaviourTree()
        {
            Builder = new TreeBuilder(this, m_Root);
        }
  
        public void Reset()
        {
            if (m_Root != null)
            {
                m_Root.Reset();
            }
        }
        public Node.NodeResult Execute()
        {
            if (m_Root.Result == Node.NodeResult.Continue)
                m_TreeState = m_Root.Execute();
            return m_TreeState;
        }     
    }
}
