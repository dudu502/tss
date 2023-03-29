using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.FSM
{
    public class StateMachine<TState,TPARAM> where TState : Enum
    {
        public static Action<string> Log;
        private readonly List<State<TState,TPARAM>> m_States = new List<State<TState,TPARAM>>();
        private State<TState, TPARAM> m_CurrentActiveState = null;
        private bool m_Running = false;
        private bool m_Inited = false;
        private readonly TPARAM m_Parameter;
        public StateMachine(TPARAM param)
        {
            m_Parameter = param;
        }

        internal TPARAM GetParameter()
        {
            return m_Parameter;
        }

        public void Start(TState startStateName)
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
        public State<TState, TPARAM> NewState(TState stateName)
        {
            State<TState, TPARAM> state = new State<TState, TPARAM>(stateName, this);
            m_States.Add(state);
            return state;
        }
        public StateMachine<TState,TPARAM> Any(TState to, Func<TPARAM, bool> valid, Action<TPARAM> transfer = null)
        {
            foreach (State<TState, TPARAM> state in m_States)
            {
                if (!Enum.Equals(to, state.Name))
                {
                    Translation<TState, TPARAM> translation = new Translation<TState, TPARAM>(state, valid).Transfer(transfer);
                    translation.To(to);
                    state.Translations.Add(translation);
                }
            }
            return this;
        }
        public StateMachine<TState, TPARAM> Initialize()
        {
            foreach (State<TState, TPARAM> state in m_States)
                state.OnInitialize();
            m_Inited = true;
            return this;
        }
        private State<TState, TPARAM> GetState(TState stateName)
        {
            foreach (State<TState, TPARAM> state in m_States)
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
                foreach (Translation<TState, TPARAM> translation in m_CurrentActiveState.Translations)
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
