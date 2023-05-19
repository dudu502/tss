using System;

namespace Task.Switch.Structure.HFSM
{
    public class Transition<TStateObject> : IDeclarative<TStateObject>
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
      
        public Transition<TStateObject> Translate(Func<TStateObject,bool> valid)
        {
            m_Valid = valid;
            return this;
        }

        public Transition<TStateObject> To(int id)
        {
            ToId = id;
            return this;
        }

        public State<TStateObject> Initialize(Action<TStateObject> init)
        {
            throw new Exception($"{nameof(Transition<TStateObject>)} {nameof(Initialize)} is Not Available");
        }

        public State<TStateObject> Enter(Action<TStateObject> enter)
        {
            throw new Exception($"{nameof(Transition<TStateObject>)} {nameof(Enter)} is Not Available");
        }

        public State<TStateObject> Update(Action<TStateObject> update)
        {
            throw new Exception($"{nameof(Transition<TStateObject>)} {nameof(Update)} is Not Available");
        }

        public State<TStateObject> Exit(Action<TStateObject> exit)
        {
            throw new Exception($"{nameof(Transition<TStateObject>)} {nameof(Exit)} is Not Available");
        }
    }
}
