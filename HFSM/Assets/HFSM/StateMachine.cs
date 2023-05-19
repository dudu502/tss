using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.HFSM
{
    public class StateMachineLogger
    {
        public static Action<string> LogInfo;
    }
    public class StateMachine<TStateObject> : State<TStateObject>
    {
        private StateMachineBuilder<TStateObject> m_Builder;

        private TStateObject m_StateObject;

        private State<TStateObject> m_CurrentState = null;
        
        private readonly Dictionary<int,State<TStateObject>> m_SubStates = new Dictionary<int, State<TStateObject>>();

        private readonly Dictionary<int,List<Transition<TStateObject>>> m_Transitions = new Dictionary<int, List<Transition<TStateObject>>>();

        private int m_DefaultTransitionToStateId;

        public StateMachine(TStateObject so)
        {
            m_StateObject = so;
     
            AddState(new State<TStateObject>(int.MaxValue));
            AddState(new State<TStateObject>(int.MinValue));
            m_CurrentState = m_SubStates[int.MaxValue];
        }

        public StateMachineBuilder<TStateObject> Builder
        {
            get 
            {
                if (m_Builder == null)
                {
                    m_Builder = new StateMachineBuilder<TStateObject>(this);
                    m_Builder.InitializeBuilderStack();
                }
                return m_Builder; 
            }
        }

        public int StateCount => m_SubStates.Count;
        public State<TStateObject> GetStateAt(int index)
        {
            return m_SubStates[index];
        }
        public TStateObject GetStateObject() { return m_StateObject; }

        public StateMachine(int stateId):base(stateId)
        {
            AddState(new State<TStateObject>(int.MaxValue));
            AddState(new State<TStateObject>(int.MinValue));
            m_CurrentState = m_SubStates[int.MaxValue];
        }

        public StateMachine<TStateObject> SetDefault(int stateId)
        {
            m_DefaultTransitionToStateId = stateId;
            AddTransition(int.MaxValue, stateId, so => true);
            return this;
        }

        public void AddTransition(int fromId,int toId, Func<TStateObject, bool> valid)
        {
            Transition<TStateObject> transition = new Transition<TStateObject>(fromId,toId,valid);
            AddTransition(transition);
        }

        public void AddTransition(Transition<TStateObject> transition)
        {
            if (!m_Transitions.ContainsKey(transition.Id))
                m_Transitions.Add(transition.Id, new List<Transition<TStateObject>>());
            m_Transitions[transition.Id].Add(transition);
        }

        public void AddState(State<TStateObject> state)
        {   
            m_SubStates.Add(state.Id,state);
        }

        public void AddState(StateMachine<TStateObject> state)
        {
            state.m_StateObject = m_StateObject;
            m_SubStates.Add(state.Id, state);
        }

        internal override void OnInitialize(TStateObject stateObject)
        {
            base.OnInitialize(stateObject);
            foreach (State<TStateObject> state in m_SubStates.Values)
                if(state.GetType() != typeof(StateMachine<TStateObject>)) 
                    state.OnInitialize(m_StateObject);
        }

        internal override void OnEnter(TStateObject stateObject)
        {
            base.OnEnter(stateObject);
            m_CurrentState = m_SubStates[int.MaxValue];
        }

        internal override void OnUpdate(TStateObject stateObject)
        {
            base.OnUpdate(stateObject);
            Update();
        }

        internal override void OnExit(TStateObject stateObject)
        {
            base.OnExit(stateObject);
            m_CurrentState = m_SubStates[int.MinValue];
        }

        public void Update()
        {
            if (m_CurrentState != null && !m_CurrentState.IsEnd() && m_Transitions.ContainsKey(m_CurrentState.Id))
            {
                foreach (Transition<TStateObject> transition in m_Transitions[m_CurrentState.Id])
                {
                    if (transition.OnValid(m_StateObject))
                    {
                        m_CurrentState.OnExit(m_StateObject);
                        m_CurrentState = m_SubStates[transition.ToId];
                        m_CurrentState.OnEnter(m_StateObject);
                        return;
                    }
                }
                m_CurrentState.OnUpdate(m_StateObject);
            }
        }
    }
}
