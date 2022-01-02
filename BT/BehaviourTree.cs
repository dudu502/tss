
using System;
using Task.Switch.Structure.BT.Actions;
using Task.Switch.Structure.BT.Composites;
using Task.Switch.Structure.BT.Decorators;

namespace Task.Switch.Structure.BT
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
        public Node Do()
        {
            return AddNodeToTree(new GenericNode());
        }
        public Node Wait(int ms,Node.NodeResult waitResult = Node.NodeResult.Success)
        {
            return AddNodeToTree(new WaitNode(ms, waitResult));
        }
        public Node WaitFrame(int frameCount,Node.NodeResult waitResult = Node.NodeResult.Success)
        {
            return AddNodeToTree(new WaitFrameNode(frameCount, waitResult));
        }
        public Node WaitUntil(Func<bool> waitFunc,Node.NodeResult nodeResult = Node.NodeResult.Success)
        {   
            return AddNodeToTree(new WaitUntilNode(waitFunc, nodeResult));
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
