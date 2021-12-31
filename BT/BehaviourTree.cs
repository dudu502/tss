using BT.Actions;
using BT.Composite;
using BT.Decorator;
using System;
using System.Collections.Generic;
using System.Text;

namespace BT
{
    public class BehaviourTree
    {
        private readonly RootNode m_Root = new RootNode();
        private Node.NodeResult m_TreeState = Node.NodeResult.Continue;
        private Node m_Pointer;
        public BehaviourTree()
        {
            m_Pointer = m_Root;
        }

        public Node.NodeResult Execute()
        {
            if (m_Root.Result == Node.NodeResult.Continue)
                m_TreeState = m_Root.Execute();
            return m_TreeState;
        }

        public BehaviourTree Repeat()
        {
            m_Pointer = AddNodeToTree(new RepeatNode());
            return this;
        }
        public BehaviourTree Sequence()
        {
            m_Pointer = AddNodeToTree(new SequencerNode());
            return this;
        }
        public BehaviourTree Select()
        {
            m_Pointer = AddNodeToTree(new SelectorNode());
            return this;
        }
        public BehaviourTree Parallel()
        {
            m_Pointer = AddNodeToTree(new ParallelNode());
            return this;
        }
        public GenericNode Do()
        {
            return AddNodeToTree(new GenericNode()) as GenericNode;
        }
        public WaitNode Wait(int ms,Node.NodeResult waitResult = Node.NodeResult.Success)
        {
            return AddNodeToTree(new WaitNode(ms, waitResult)) as WaitNode;
        }
        public WaitUntilNode WaitUntil(Func<bool> waitFunc,Node.NodeResult nodeResult = Node.NodeResult.Success)
        {   
            return AddNodeToTree(new WaitUntilNode(waitFunc, nodeResult)) as WaitUntilNode;
        }

        Node AddNodeToTree(Node target)
        {
            if(m_Pointer!=null)
            {
                if(m_Pointer is RootNode)
                {
                    ((RootNode)m_Pointer).SetChild(target);
                }
                else if(m_Pointer is DecoratorNode)
                {
                    ((DecoratorNode)m_Pointer).SetChild(target);
                }
                else if(m_Pointer is CompositeNode)
                {
                    ((CompositeNode)m_Pointer).AddChild(target);
                }
            }
            target.Tree = this;
            return target;
        }

    }
}
