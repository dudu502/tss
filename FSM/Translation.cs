using System;
namespace Task.Switch.Structure.FSM
{
    public class Translation<T>where T:Enum
    {
        private readonly Func<bool> m_Valid;
        private Action m_Transfer;
        internal T ToStateName { private set; get; }
        private readonly State<T> m_Current;
        public Translation(State<T> state,Func<bool> valid,Action transfer)
        {
            m_Current = state;
            m_Valid = valid;
            m_Transfer = transfer;
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
            {
                valid = m_Valid();
            }
            if (StateMachine<T>.Logger != null && StateMachine<T>.Logger.IsDebugEnabled)
                StateMachine<T>.Logger.Debug($"State:{m_Current.Name} ToState:{ToStateName} OnValid {valid}");
            return valid;
        }

        internal void OnTransfer()
        {
            if(m_Transfer != null)
            {
                if (StateMachine<T>.Logger != null && StateMachine<T>.Logger.IsDebugEnabled)
                    StateMachine<T>.Logger.Debug($"State:{m_Current.Name} ToState:{ToStateName} OnTransfer");
                m_Transfer();
            }
        }
    }
}
