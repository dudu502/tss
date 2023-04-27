using System;
namespace Task.Switch.Structure.FSM
{
    public class Translation<TState, TParam>    where TState : Enum 
                                                where TParam : class
    {
        private readonly Func<TParam, bool> m_Valid;
        private Action<TParam> m_Transfer;
        internal TState ToStateName { private set; get; }
        private readonly State<TState, TParam> m_State;
        public Translation(State<TState, TParam> state, Func<TParam, bool> valid)
        {
            m_State = state;
            m_Valid = valid;
        }

        public static Translation<TState, TParam> Clone(Translation<TState, TParam> original, State<TState, TParam> state)
        {
            Translation<TState, TParam> clone = new Translation<TState, TParam>(state, original.m_Valid);
            clone.m_Transfer = original.m_Transfer;
            clone.ToStateName = original.ToStateName;
            return clone;
        }
        public State<TState, TParam> To(TState stateName)
        {
            ToStateName = stateName;
            return m_State;
        }
        internal bool OnValid()
        {
            bool valid = false;
            if (m_Valid != null)
                valid = m_Valid(m_State.GetParameter());
            if (StateMachine<TState, TParam>.Log != null)
                StateMachine<TState, TParam>.Log($"State:{m_State.Name} {nameof(OnValid)}:{valid} ToState:{ToStateName}");
            return valid;
        }
        public Translation<TState, TParam> Transfer(Action<TParam> transfer)
        {
            m_Transfer = transfer;
            return this;
        }
        internal void OnTransfer()
        {
            if (m_Transfer != null)
            {
                if (StateMachine<TState, TParam>.Log != null)
                    StateMachine<TState, TParam>.Log($"State:{m_State.Name} {nameof(OnTransfer)} ToState:{ToStateName}");
                m_Transfer(m_State.GetParameter());
            }
        }
    }
}
