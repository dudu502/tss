using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.FSM
{
    public class StateMachine<T> where T : Enum
    {
        public static Action<string> Log;
        private readonly List<State<T>> m_States = new List<State<T>>();
        private State<T> m_CurrentActiveState = null;
        private bool m_Running = false;
        private bool m_Inited = false;
        private readonly object m_Parameter;
        public StateMachine(object param)
        {
            m_Parameter = param;
        }

        public PARAM GetParameter<PARAM>()
        {
            if (m_Parameter != null)
                return (PARAM)m_Parameter;
            return default(PARAM);
        }

        public void Start(T startStateName)
        {
            if (m_Inited)
            {
                m_Running = true;
                m_CurrentActiveState = GetState(startStateName);
                m_CurrentActiveState.OnEnter();
            }
        }
        public void Stop()
        {
            m_Running = false;
            m_Inited = false;
            Log = null;
            m_States.Clear();
            m_CurrentActiveState = null;
        }
        public void Pause()
        {
            m_Running = false;
        }
        public void Resume()
        {
            m_Running = true;
        }
        public State<T> NewState(T stateName)
        {
            State<T> state = new State<T>(stateName, this);
            m_States.Add(state);
            return state;
        }
        public StateMachine<T> Any(T to, Func<bool> valid, Action transfer = null)
        {
            foreach (State<T> state in m_States)
            {
                if (!Enum.Equals(to, state.Name))
                {
                    Translation<T> translation = new Translation<T>(state, valid).Transfer(transfer);
                    translation.To(to);
                    state.Translations.Add(translation);
                }
            }
            return this;
        }
        public StateMachine<T> Initialize()
        {
            foreach (State<T> state in m_States)
                state.OnInitialize();
            m_Inited = true;
            return this;
        }
        private State<T> GetState(T stateName)
        {
            foreach (State<T> state in m_States)
            {
                if (Enum.Equals(state.Name, stateName))
                    return state;
            }
            throw new Exception($"{stateName} is not exist! Please call NewState to create this state");
        }
        public void Update()
        {
            if (m_Running && m_CurrentActiveState != null)
            {
                foreach (Translation<T> translation in m_CurrentActiveState.Translations)
                {
                    if (translation.OnValid())
                    {
                        m_CurrentActiveState.OnExit();
                        m_CurrentActiveState = GetState(translation.ToStateName);
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
