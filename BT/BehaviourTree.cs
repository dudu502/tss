
using System;
using System.Collections.Generic;
using Task.Switch.Structure.BT.Actions;
using Task.Switch.Structure.BT.Composites;
using Task.Switch.Structure.BT.Decorators;

namespace Task.Switch.Structure.BT
{
    public class BehaviourTree
    {
        private readonly RootNode m_Root = new RootNode();
        private readonly Stack<Node> m_PointerStack = new Stack<Node>();
        private Node.NodeResult m_TreeState = Node.NodeResult.Continue;
        public BehaviourTree()
        {
            m_PointerStack.Push(m_Root);
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

        public BehaviourTree Repeat()
        {
            PushNodeToTree(new RepeatNode());
            return this;
        }
        public BehaviourTree Sequence()
        {
            PushNodeToTree(new SequencerNode());
            return this;
        }
        public BehaviourTree Select()
        {
            PushNodeToTree(new SelectorNode());
            return this;
        }
        public BehaviourTree Parallel()
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
        public Node Wait(int ms,Node.NodeResult waitResult = Node.NodeResult.Success)
        {
            WaitNode waitNode = new WaitNode(ms, waitResult);
            PushNodeToTree(waitNode);
            return waitNode;
        }
        public Node WaitFrame(int frameCount,Node.NodeResult waitResult = Node.NodeResult.Success)
        {
            WaitFrameNode waitFrameNode = new WaitFrameNode(frameCount, waitResult);
            PushNodeToTree(waitFrameNode);
            return waitFrameNode;
        }
        public Node WaitUntil(Func<bool> waitFunc,Node.NodeResult nodeResult = Node.NodeResult.Success)
        {   
            WaitUntilNode waitUntilNode = new WaitUntilNode(waitFunc,nodeResult);
            PushNodeToTree(waitUntilNode);
            return waitUntilNode;
        }
        public BehaviourTree End()
        {
            if(m_PointerStack.Count > 0)
                m_PointerStack.Pop();
            return this;
        }
        void PushNodeToTree(Node child)
        {
            child.Tree = this;
            Node parent = m_PointerStack.Peek();
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
            m_PointerStack.Push(child);
        }
    }
}
