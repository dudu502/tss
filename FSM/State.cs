using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.FSM
{
    public class State<T,PARAM> where T:Enum
    {
        internal readonly T Name;
        readonly StateMachine<T, PARAM> m_Machine;
        internal readonly List<Translation<T, PARAM>> Translations = new List<Translation<T, PARAM>>();
        private Action<PARAM> m_OnInitialize;
        private Action<PARAM> m_OnEnter;
        private Action<PARAM> m_OnUpdate;
        private Action<PARAM> m_OnExit;

        public State(T name,StateMachine<T, PARAM> stateMachine)
        {
            Name = name;
            m_Machine = stateMachine;
        }
        internal PARAM GetParameter()
        {
            return m_Machine.GetParameter();
        }
        public StateMachine<T, PARAM> End()
        {
            return m_Machine;
        }
        public Translation<T, PARAM> Translate(Func<PARAM, bool> valid)
        {
            Translation<T, PARAM> translation = new Translation<T, PARAM>(this, valid);
            Translations.Add(translation);
            return translation;
        }
        public State<T, PARAM> Initialize(Action<PARAM> init) { m_OnInitialize = init; return this; }
        public State<T, PARAM> Enter(Action<PARAM> enter) { m_OnEnter = enter; return this; }
        public State<T, PARAM> Update(Action<PARAM> update) { m_OnUpdate = update; return this; }
        public State<T, PARAM> Exit(Action<PARAM> exit) { m_OnExit = exit; return this; }
        internal void OnInitialize()
        {
            if (m_OnInitialize != null)
            {
                if (StateMachine<T, PARAM>.Log != null)
                    StateMachine<T, PARAM>.Log($"{Name} OnInitialize");
                m_OnInitialize(GetParameter());
            }
        }
        internal void OnEnter()
        {
            if (m_OnEnter != null)
            {
                if (StateMachine<T, PARAM>.Log != null)
                    StateMachine<T, PARAM>.Log($"{Name} m_OnEnter");
                m_OnEnter(GetParameter());
            }
        }

        internal void OnUpdate()
        {
            if (m_OnUpdate != null)
            {
                if (StateMachine<T, PARAM>.Log != null)
                    StateMachine<T, PARAM>.Log($"{Name} OnUpdate");
                m_OnUpdate(GetParameter());
            }
        }
        internal void OnExit()
        {
            if (m_OnExit != null)
            {
                if (StateMachine<T, PARAM>.Log != null)
                    StateMachine<T, PARAM>.Log($"{Name} OnExit");
                m_OnExit(GetParameter());
            }
        }
    }
}
