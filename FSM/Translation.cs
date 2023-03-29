using System;
namespace Task.Switch.Structure.FSM
{
    public class Translation<TState,TPARAM> where TState:Enum
    {
        private readonly Func<TPARAM,bool> m_Valid;
        private Action<TPARAM> m_Transfer;
        internal TState ToStateName { private set; get; }
        private readonly State<TState, TPARAM> m_Current;
        public Translation(State<TState, TPARAM> state,Func<TPARAM,bool> valid)
        {
            m_Current = state;
            m_Valid = valid;
        }
        public State<TState, TPARAM> To(TState stateName)
        {
            ToStateName = stateName;
            return m_Current;
        }
        internal bool OnValid()
        {
            bool valid = false;
            if (m_Valid != null)
                valid = m_Valid(m_Current.GetParameter());
            if (StateMachine<TState, TPARAM>.Log != null)
                StateMachine<TState, TPARAM>.Log($"State:{m_Current.Name} ToState:{ToStateName} OnValid {valid}");
            return valid;
        }
        public Translation<TState, TPARAM> Transfer(Action<TPARAM> transfer)
        {
            m_Transfer = transfer;
            return this;
        }
        internal void OnTransfer()
        {
            if (m_Transfer != null)
            {
                if (StateMachine<TState, TPARAM>.Log != null)
                    StateMachine<TState, TPARAM>.Log($"State:{m_Current.Name} ToState:{ToStateName} OnTransfer");
                m_Transfer(m_Current.GetParameter());
            }
        }
    }
}
