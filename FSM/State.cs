using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.FSM
{
    public class State<TState,TPARAM> where TState:Enum
    {
        internal readonly TState Name;
        readonly StateMachine<TState, TPARAM> m_Machine;
        internal readonly List<Translation<TState, TPARAM>> Translations = new List<Translation<TState, TPARAM>>();
        private Action<TPARAM> m_OnInitialize;
        private Action<TPARAM> m_OnEnter;
        private Action<TPARAM> m_OnUpdate;
        private Action<TPARAM> m_OnExit;

        public State(TState name,StateMachine<TState, TPARAM> stateMachine)
        {
            Name = name;
            m_Machine = stateMachine;
        }
        internal TPARAM GetParameter()
        {
            return m_Machine.GetParameter();
        }
        public StateMachine<TState, TPARAM> End()
        {
            return m_Machine;
        }
        public Translation<TState, TPARAM> Translate(Func<TPARAM, bool> valid)
        {
            Translation<TState, TPARAM> translation = new Translation<TState, TPARAM>(this, valid);
            Translations.Add(translation);
            return translation;
        }
        public State<TState, TPARAM> Initialize(Action<TPARAM> init) { m_OnInitialize = init; return this; }
        public State<TState, TPARAM> Enter(Action<TPARAM> enter) { m_OnEnter = enter; return this; }
        public State<TState, TPARAM> Update(Action<TPARAM> update) { m_OnUpdate = update; return this; }
        public State<TState, TPARAM> Exit(Action<TPARAM> exit) { m_OnExit = exit; return this; }
        internal void OnInitialize()
        {
            if (m_OnInitialize != null)
            {
                if (StateMachine<TState, TPARAM>.Log != null)
                    StateMachine<TState, TPARAM>.Log($"{Name} OnInitialize");
                m_OnInitialize(GetParameter());
            }
        }
        internal void OnEnter()
        {
            if (m_OnEnter != null)
            {
                if (StateMachine<TState, TPARAM>.Log != null)
                    StateMachine<TState, TPARAM>.Log($"{Name} m_OnEnter");
                m_OnEnter(GetParameter());
            }
        }

        internal void OnUpdate()
        {
            if (m_OnUpdate != null)
            {
                if (StateMachine<TState, TPARAM>.Log != null)
                    StateMachine<TState, TPARAM>.Log($"{Name} OnUpdate");
                m_OnUpdate(GetParameter());
            }
        }
        internal void OnExit()
        {
            if (m_OnExit != null)
            {
                if (StateMachine<TState, TPARAM>.Log != null)
                    StateMachine<TState, TPARAM>.Log($"{Name} OnExit");
                m_OnExit(GetParameter());
            }
        }
    }
}
