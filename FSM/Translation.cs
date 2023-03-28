﻿using System;
namespace Task.Switch.Structure.FSM
{
    public class Translation<T> where T:Enum
    {
        private readonly Func<bool> m_Valid;
        private Action m_Transfer;
        internal T ToStateName { private set; get; }
        private readonly State<T> m_Current;
        public Translation(State<T> state,Func<bool> valid)
        {
            m_Current = state;
            m_Valid = valid;
        }
        public State<T> To(T stateName)
        {
            ToStateName = stateName;
            return m_Current;
        }
        internal bool OnValid()
        {
            bool valid = false;
            if (m_Valid != null)
                valid = m_Valid();
            if (StateMachine<T>.Log != null)
                StateMachine<T>.Log($"State:{m_Current.Name} ToState:{ToStateName} OnValid {valid}");
            return valid;
        }
        public Translation<T> Transfer(Action transfer)
        {
            m_Transfer = transfer;
            return this;
        }
        internal void OnTransfer()
        {
            if (m_Transfer != null)
            {
                if (StateMachine<T>.Log != null)
                    StateMachine<T>.Log($"State:{m_Current.Name} ToState:{ToStateName} OnTransfer");
                m_Transfer();
            }
        }
    }
}
