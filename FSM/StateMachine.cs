using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.FSM
{
    public class StateMachine<T,PARAM> where T : Enum
    {
        public static Action<string> Log;
        private readonly List<State<T,PARAM>> m_States = new List<State<T,PARAM>>();
        private State<T, PARAM> m_CurrentActiveState = null;
        private bool m_Running = false;
        private bool m_Inited = false;
        private readonly PARAM m_Parameter;
        public StateMachine(PARAM param)
        {
            m_Parameter = param;
        }

        internal PARAM GetParameter()
        {
            return m_Parameter;
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
        public State<T, PARAM> NewState(T stateName)
        {
            State<T, PARAM> state = new State<T, PARAM>(stateName, this);
            m_States.Add(state);
            return state;
        }
        public StateMachine<T,PARAM> Any(T to, Func<PARAM, bool> valid, Action<PARAM> transfer = null)
        {
            foreach (State<T, PARAM> state in m_States)
            {
                if (!Enum.Equals(to, state.Name))
                {
                    Translation<T, PARAM> translation = new Translation<T, PARAM>(state, valid).Transfer(transfer);
                    translation.To(to);
                    state.Translations.Add(translation);
                }
            }
            return this;
        }
        public StateMachine<T, PARAM> Initialize()
        {
            foreach (State<T, PARAM> state in m_States)
                state.OnInitialize();
            m_Inited = true;
            return this;
        }
        private State<T, PARAM> GetState(T stateName)
        {
            foreach (State<T, PARAM> state in m_States)
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
                foreach (Translation<T, PARAM> translation in m_CurrentActiveState.Translations)
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
