using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.FSM
{
    public class State<T> where T:Enum
    {
        internal readonly T Name;
        readonly StateMachine<T> m_Machine;
        internal readonly List<Translation<T>> Translations = new List<Translation<T>>();
        private Action m_OnInitialize;
        private Action m_OnEnter;
        private Action m_OnUpdate;
        private Action m_OnExit;

        public State(T name,StateMachine<T> stateMachine)
        {
            Name = name;
            m_Machine = stateMachine;
        }

        public StateMachine<T> End()
        {
            return m_Machine;
        }
        public Translation<T> Translate(Func<bool> valid)
        {
            Translation<T> translation = new Translation<T>(this, valid);
            Translations.Add(translation);
            return translation;
        }
        public State<T> Initialize(Action init) { m_OnInitialize = init; return this; }
        public State<T> Enter(Action enter) { m_OnEnter = enter; return this; }
        public State<T> Update(Action update) { m_OnUpdate = update; return this; }
        public State<T> Exit(Action exit) { m_OnExit = exit; return this; }
        internal void OnInitialize()
        {
            if (m_OnInitialize != null)
            {
                if (StateMachine<T>.Logger != null && StateMachine<T>.Logger.IsDebugEnabled)
                    StateMachine<T>.Logger.Debug($"{Name} OnInitialize");
                m_OnInitialize();
            }
        }
        internal void OnEnter()
        {
            if (m_OnEnter != null)
            {
                if (StateMachine<T>.Logger != null && StateMachine<T>.Logger.IsDebugEnabled)
                    StateMachine<T>.Logger.Debug($"{Name} m_OnEnter");
                m_OnEnter();
            }
        }
        internal void OnUpdate()
        {
            if (m_OnUpdate != null)
            {
                if (StateMachine<T>.Logger != null && StateMachine<T>.Logger.IsDebugEnabled)
                    StateMachine<T>.Logger.Debug($"{Name} OnUpdate");
                m_OnUpdate();
            }
        }
        internal void OnExit()
        {
            if (m_OnExit != null)
            {
                if (StateMachine<T>.Logger != null && StateMachine<T>.Logger.IsDebugEnabled)
                    StateMachine<T>.Logger.Debug($"{Name} OnExit");
                m_OnExit();
            }
        }
    }
}
