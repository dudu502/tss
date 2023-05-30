using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.HFSM
{
    public class State<TStateObject>
    {
        private Action<TStateObject> m_OnInitialize;
        private Action<TStateObject> m_OnEnter;
        private Action<TStateObject> m_OnUpdate;
        private Action<TStateObject> m_OnExit;
        public int Id { set; get; }

        public State(int id)
        {
            Id = id;
        }

        public State<TStateObject> Initialize(Action<TStateObject> init) { m_OnInitialize = init; return this; }
        public State<TStateObject> Enter(Action<TStateObject> enter) { m_OnEnter = enter; return this; }
        public State<TStateObject> Update(Action<TStateObject> update) { m_OnUpdate = update; return this; }
        public State<TStateObject> Exit(Action<TStateObject> exit) { m_OnExit = exit; return this; }

        internal bool IsEnd()
        {
            return Id == int.MinValue;
        }

        internal bool IsEntry()
        {
            return Id == int.MaxValue;
        }

        internal virtual void OnInitialize(TStateObject stateObject)
        {
            if (m_OnInitialize != null)
            {
                StateMachineLogger.LogInfo?.Invoke($"{Id} OnInitialize");
                m_OnInitialize(stateObject);
            }
        }

        internal virtual void OnEnter(TStateObject stateObject)
        {
            if (m_OnEnter != null)
            {
                StateMachineLogger.LogInfo?.Invoke($"{Id} OnEnter");
                m_OnEnter(stateObject);
            }
        }

        internal virtual void OnUpdate(TStateObject stateObject)
        {
            if (m_OnUpdate != null)
            {
                StateMachineLogger.LogInfo?.Invoke($"{Id} OnUpdate");
                m_OnUpdate(stateObject);
            }
        }

        internal virtual void OnExit(TStateObject stateObject)
        {
            if (m_OnExit != null)
            {
                StateMachineLogger.LogInfo?.Invoke($"{Id} OnExit");
                m_OnExit(stateObject);
            }
        }
    }
}
