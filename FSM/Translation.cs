﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FSM
{
    public class Translation<T>where T:Enum
    {
        public string Name { set; get; }
        private Func<bool> m_Valid;
        private Action m_Transfer;
        public T ToStateName { private set; get; }
        private readonly State<T> m_Current;
        public Translation(State<T> state)
        {
            m_Current = state;
        }
        public Translation<T> Valid(Func<bool> valid)
        {
            m_Valid = valid;
            return this;
        }
        public State<T> To(T stateName)
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
        public Translation<T> Transfer(Action transfer)
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