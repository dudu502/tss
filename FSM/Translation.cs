using System;
namespace Task.Switch.Structure.FSM
{
    public class Translation<T,PARAM> where T:Enum
    {
        private readonly Func<PARAM,bool> m_Valid;
        private Action<PARAM> m_Transfer;
        internal T ToStateName { private set; get; }
        private readonly State<T, PARAM> m_Current;
        public Translation(State<T, PARAM> state,Func<PARAM,bool> valid)
        {
            m_Current = state;
            m_Valid = valid;
        }
        public State<T, PARAM> To(T stateName)
        {
            ToStateName = stateName;
            return m_Current;
        }
        internal bool OnValid()
        {
            bool valid = false;
            if (m_Valid != null)
                valid = m_Valid(m_Current.GetParameter());
            if (StateMachine<T, PARAM>.Log != null)
                StateMachine<T, PARAM>.Log($"State:{m_Current.Name} ToState:{ToStateName} OnValid {valid}");
            return valid;
        }
        public Translation<T, PARAM> Transfer(Action<PARAM> transfer)
        {
            m_Transfer = transfer;
            return this;
        }
        internal void OnTransfer()
        {
            if (m_Transfer != null)
            {
                if (StateMachine<T, PARAM>.Log != null)
                    StateMachine<T, PARAM>.Log($"State:{m_Current.Name} ToState:{ToStateName} OnTransfer");
                m_Transfer(m_Current.GetParameter());
            }
        }
    }
}
