using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.FSM
{
    public class State<TState, TParam> where TState : Enum where TParam : class
    {
        internal readonly TState Name;
        readonly StateMachine<TState, TParam> m_Machine;
        internal readonly List<Translation<TState, TParam>> Translations = new List<Translation<TState, TParam>>();
        private Action<TParam> m_OnInitialize;
        private Action<TParam> m_OnEnter;
        private Action<TParam> m_OnUpdate;
        private Action<TParam> m_OnExit;


        internal static State<TState, TParam> Clone(State<TState, TParam> origin, StateMachine<TState, TParam> stateMachine)
        {
            State<TState, TParam> cloned = new State<TState, TParam>(origin.Name, stateMachine);
            cloned.m_OnInitialize = origin.m_OnInitialize;
            cloned.m_OnEnter = origin.m_OnEnter;
            cloned.m_OnUpdate = origin.m_OnUpdate;
            cloned.m_OnExit = origin.m_OnExit;
            foreach (Translation<TState, TParam> translation in origin.Translations)
                cloned.Translations.Add(Translation<TState, TParam>.Clone(translation, cloned));
            return cloned;
        }

        public State(TState name, StateMachine<TState, TParam> stateMachine)
        {
            Name = name;
            m_Machine = stateMachine;
        }

        internal TParam GetParameter()
        {
            return m_Machine.GetParameter();
        }
        public StateMachine<TState, TParam> End()
        {
            return m_Machine;
        }
        public Translation<TState, TParam> Translate(Func<TParam, bool> valid)
        {
            Translation<TState, TParam> translation = new Translation<TState, TParam>(this, valid);
            Translations.Add(translation);
            return translation;
        }
        public State<TState, TParam> Initialize(Action<TParam> init) { m_OnInitialize = init; return this; }
        public State<TState, TParam> Enter(Action<TParam> enter) { m_OnEnter = enter; return this; }
        public State<TState, TParam> Update(Action<TParam> update) { m_OnUpdate = update; return this; }
        public State<TState, TParam> Exit(Action<TParam> exit) { m_OnExit = exit; return this; }
        internal void OnInitialize()
        {
            if (m_OnInitialize != null)
            {
                if (StateMachine<TState, TParam>.Log != null)
                    StateMachine<TState, TParam>.Log($"{Name} {nameof(OnInitialize)}");
                m_OnInitialize(GetParameter());
            }
        }
        internal void OnEnter()
        {
            if (m_OnEnter != null)
            {
                if (StateMachine<TState, TParam>.Log != null)
                    StateMachine<TState, TParam>.Log($"{Name} {nameof(OnEnter)}");
                m_OnEnter(GetParameter());
            }
        }

        internal void OnUpdate()
        {
            if (m_OnUpdate != null)
            {
                if (StateMachine<TState, TParam>.Log != null)
                    StateMachine<TState, TParam>.Log($"{Name} {nameof(OnUpdate)}");
                m_OnUpdate(GetParameter());
            }
        }
        internal void OnExit()
        {
            if (m_OnExit != null)
            {
                if (StateMachine<TState, TParam>.Log != null)
                    StateMachine<TState, TParam>.Log($"{Name} {nameof(OnExit)}");
                m_OnExit(GetParameter());
            }
        }
    }
}
