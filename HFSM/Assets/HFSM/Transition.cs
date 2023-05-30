using System;

namespace Task.Switch.Structure.HFSM
{
    public class Transition<TStateObject>
    {
        private Func<TStateObject, bool> m_Valid;

        public int Id { set; get; }
        internal int ToId;

        public Transition(int fromId,int toId, Func<TStateObject, bool> valid)
        {
            Id = fromId;
            ToId = toId;
            m_Valid = valid;
        }
        
        internal bool OnValid(TStateObject stateObject)
        {
            bool validity = false;
            if (m_Valid != null)
                validity = m_Valid(stateObject);
            StateMachineLogger.LogInfo?.Invoke($"From:{Id} To:{ToId} OnValid Result:{validity}");
            return validity;
        }
    }
}
