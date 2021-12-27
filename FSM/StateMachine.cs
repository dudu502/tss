using System;
using System.Collections.Generic;
using System.Text;

namespace FSM
{
    public class StateMachine
    {
        private List<State> m_States;
        private State m_CurrentActiveState;
        private bool m_Running = false;
        public StateMachine()
        {
            m_CurrentActiveState = null;
            m_States = new List<State>();
        }
        public void Start(string startStateName)
        {
            m_Running = true;
            m_CurrentActiveState = Find(startStateName);
            m_CurrentActiveState.OnEnter();
        }
        public void Stop()
        {
            m_Running = false;
        }
        public State NewState(string stateName)
        {
            State state = new State();
            state.Name = stateName;
            m_States.Add(state);
            return state;
        }
        public void Initialize()
        {
            foreach (State state in m_States)
                state.OnInitialize();
        }
        private State Find(string stateName)
        {
            foreach(State state in m_States)
            {
                if (state.Name == stateName)
                    return state;
            }
            throw new Exception($"{stateName} is not exist! Please call NewState to create this state");
        }
        public void Update()
        {
            if (m_Running && m_CurrentActiveState != null)
            {
                foreach (Translation translation in m_CurrentActiveState.Translations)
                {
                    if (translation.OnValid())
                    {
                        m_CurrentActiveState.OnExit();
                        m_CurrentActiveState = Find(translation.ToStateName);
                        translation.OnTransfer();
                        m_CurrentActiveState.OnEnter();
                        return;
                    }
                }
                m_CurrentActiveState.OnUpdate();
            }
        }
    }
}
