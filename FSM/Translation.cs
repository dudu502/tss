using System;
using System.Collections.Generic;
using System.Text;

namespace FSM
{
    public class Translation
    {
        public string Name { set; get; }
        private Func<bool> m_Valid;
        private Action m_Transfer;
        public string ToStateName { private set; get; }
        private State m_Current;
        public Translation(State state)
        {
            m_Current = state;
        }
        public Translation Valid(Func<bool> valid)
        {
            m_Valid = valid;
            return this;
        }
        public State To(string stateName)
        {
            ToStateName = stateName;
            return m_Current;
        }
        public bool OnValid()
        {
            if (m_Valid != null)
                return m_Valid();
            return false;
        }
        public Translation Transfer(Action transfer)
        {
            m_Transfer = transfer;
            return this;
        }
        public void OnTransfer()
        {
            m_Transfer?.Invoke();
        }

    }
}
