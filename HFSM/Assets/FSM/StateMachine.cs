using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.FSM
{
    public class FsmEvent
    {
        public string EventType { private set; get; }
        public object Data { private set; get; }
        public FsmEvent(string eventType, object data)
        {
            EventType = eventType;
            if (string.IsNullOrEmpty(EventType))
                throw new NullReferenceException("EventType is NULL!");
            Data = data;
        }

        public FsmEvent(string eventType)
        {
            EventType = eventType;
            if (string.IsNullOrEmpty(EventType))
                throw new NullReferenceException();
            Data = null;
        }

        public T DataAs<T>()
        {
            return (T)Data;
        }

        public override string ToString()
        {
            return $"[{EventType}] {Data ?? "<null>"}";
        }
    }

    public class StateMachineDebug
    {
        public enum LogFilter
        {
            Nothing = 0,
            OnInitialize = 1,
            OnEnter = 2,
            OnEarlyUpdate = 4,
            OnUpdate = 8,
            OnExit = 16,
            OnValidate = 32,
            OnTransfer = 64,
            OnEvent = 128,
            Everything = OnInitialize | OnEnter | OnEarlyUpdate | OnUpdate | OnExit | OnValidate | OnTransfer | OnEvent,
        }
        public static Action<string> Log;
        public static LogFilter Filter = LogFilter.Nothing;
    }

    public class TransitionBase<TObject>
    {
        public int Id { get; private set; }
        public int ToId { get; private set; }
        public string EventType { get; private set; }
        private Func<TObject, bool> m_Validate;
        private Action<TObject> m_Transfer;
        private Func<TObject, FsmEvent, bool> m_OnEvent;
        private StateBase<TObject> m_State;

        public TransitionBase(int id, int toId, Func<TObject, bool> valid, Action<TObject> transfer, StateBase<TObject> stateBase)
        {
            m_State = stateBase;
            Id = id;
            ToId = toId;
            m_Validate = valid;
            m_Transfer = transfer;
            EventType = null;
        }
        public TransitionBase(int id, string type, Func<TObject, FsmEvent, bool> onEvt, StateBase<TObject> stateBase)
        {
            m_State = stateBase;
            m_OnEvent = onEvt;
            Id = id;
            EventType = type;
        }

        internal bool OnValidate(TObject param)
        {
            bool validate = false;
            if (m_Validate != null)
            {
                validate = m_Validate(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnValidate == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnValidate))
                    StateMachineDebug.Log($"<color=cyan>Transition:{Id}->{ToId} {nameof(OnValidate)} Validate:{validate} Parameters:{param}</color>");
            }
            return validate;
        }
        internal void OnTransfer(TObject param)
        {
            if (m_Transfer != null)
            {
                m_Transfer(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnTransfer == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnTransfer))
                    StateMachineDebug.Log($"<color=white>Transition:{Id}->{ToId} {nameof(OnTransfer)} Parameters:{param}</color>");
            }
        }

        internal bool OnEvent(TObject param, FsmEvent fsmEvent)
        {
            bool result = false;
            if (m_OnEvent != null)
            {
                result = m_OnEvent(param, fsmEvent);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnEvent == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnEvent))
                    StateMachineDebug.Log($"<color=cyan>Event:{Id}->{ToId} {nameof(OnEvent)} OnEventResult:{result} Parameters:{param}</color>");
            }
            return result;
        }

        public TransitionBase<TObject> Transfer(Action<TObject> transfer)
        {
            m_Transfer = transfer;
            return this;
        }
        public StateBase<TObject> Return()
        {
            ToId = -1;
            return m_State;
        }
        public StateBase<TObject> To<TState>(TState toId) where TState : Enum
        {
            ToId = Convert.ToInt32(toId);
            return m_State;
        }

        public StateBase<TObject> ToEnd()
        {
            ToId = StateMachineConst.END;
            return m_State;
        }

        public StateBase<TObject> ToEntry()
        {
            ToId = StateMachineConst.ENTRY;
            return m_State;
        }

        public void Dispose()
        {
            m_Validate = null;
            m_Transfer = null;
            m_OnEvent = null;
            m_State = null;
            EventType = null;
        }
    }

    public class StateBase<TObject>
    {
        private StateMachine<TObject> m_Parent;
        public int Id { get; private set; }
        public int PreviousId;
        protected Action<TObject> m_OnInitialize;
        protected Action<TObject> m_OnEnter;
        protected Action<TObject> m_OnUpdate;
        protected Action<TObject> m_OnExit;
        protected Action<TObject> m_OnEarlyUpdate;

        public virtual StateBase<TObject> Initialize(Action<TObject> init)
        {
            m_OnInitialize = init;
            return this;
        }

        public virtual StateBase<TObject> Enter(Action<TObject> enter)
        {
            m_OnEnter = enter;
            return this;
        }
        public virtual StateBase<TObject> Update(Action<TObject> update)
        {
            m_OnUpdate = update;
            return this;
        }
        public virtual StateBase<TObject> EarlyUpdate(Action<TObject> earlyUpdate)
        {
            m_OnEarlyUpdate = earlyUpdate;
            return this;
        }
        public virtual StateBase<TObject> Exit(Action<TObject> exit)
        {
            m_OnExit = exit;
            return this;
        }

        internal virtual void OnEnter(TObject param)
        {
            if (m_OnEnter != null)
            {
                m_OnEnter(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnEnter == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnEnter))
                    StateMachineDebug.Log($"<color=magenta>StateId:{Id} {nameof(OnEnter)} Parameters:{param}</color>");
            }
        }
        internal virtual void OnEarlyUpdate(TObject param)
        {
            if (m_OnEarlyUpdate != null)
            {
                m_OnEarlyUpdate(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnEarlyUpdate == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnEarlyUpdate))
                    StateMachineDebug.Log($"<color=#ff8000>StateId:{Id} {nameof(OnEarlyUpdate)} Parameters:{param}</color>");
            }
        }
        internal virtual void OnUpdate(TObject param)
        {
            if (m_OnUpdate != null)
            {
                m_OnUpdate(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnUpdate == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnUpdate))
                    StateMachineDebug.Log($"<color=yellow>StateId:{Id} {nameof(OnUpdate)} Parameters:{param}</color>");
            }
        }
        internal virtual void OnExit(TObject param)
        {
            if (m_OnExit != null)
            {
                m_OnExit(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnExit == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnExit))
                    StateMachineDebug.Log($"<color=red>StateId:{Id} {nameof(OnExit)} Parameters:{param}</color>");
            }
        }
        internal virtual void OnInitialize(TObject param)
        {
            if (m_OnInitialize != null)
            {
                m_OnInitialize(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnInitialize == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnInitialize))
                    StateMachineDebug.Log($"<color=green>StateId:{Id} {nameof(OnInitialize)} Parameters:{param}</color>");
            }
        }
        public StateBase(int id, StateMachine<TObject> stateMachine)
        {
            Id = id;
            m_Parent = stateMachine;
        }

        public TransitionBase<TObject> Transition(Func<TObject, bool> valid)
        {
            TransitionBase<TObject> transition = new TransitionBase<TObject>(Id, 0, valid, null, this);
            m_Parent.AddTransition(transition);
            return transition;
        }

        public TransitionBase<TObject> Event(string type, Func<TObject, FsmEvent, bool> onEvt)
        {
            TransitionBase<TObject> transition = new TransitionBase<TObject>(Id, type, onEvt, this);
            m_Parent.AddTransition(transition);
            return transition;
        }

        public StateMachine<TObject> End()
        {
            return m_Parent;
        }

        public virtual void Dispose()
        {
            m_OnEarlyUpdate = null;
            m_OnEnter = null;
            m_OnUpdate = null;
            m_OnExit = null;
            m_OnInitialize = null;
            m_Parent = null;
        }
    }

    internal class StateMachineConst
    {
        public const int ENTRY = int.MaxValue - 1;
        public const int END = int.MaxValue;
    }
    public class StateMachine<TObject> : StateBase<TObject>
    {
        Dictionary<int, StateBase<TObject>> m_States;
        Dictionary<int, List<TransitionBase<TObject>>> m_Transitions;
        StateBase<TObject> m_Current;
        TObject m_Parameter;

        public StateMachine(TObject param, int id, StateMachine<TObject> parent) : base(id, parent)
        {
            _Initialize(param);
        }

        public StateMachine(TObject param) : base(int.MinValue, null)
        {
            _Initialize(param);
        }

        public void Reset()
        {
            m_Current = m_States[StateMachineConst.ENTRY];
        }

        internal void AddTransition(TransitionBase<TObject> transition)
        {
            if (!m_Transitions.ContainsKey(transition.Id))
                m_Transitions[transition.Id] = new List<TransitionBase<TObject>>();
            m_Transitions[transition.Id].Add(transition);
        }

        public void SetParameter(TObject param)
        {
            if (param == null)
                throw new ArgumentNullException("StateMachine(TObject param)");
            m_Parameter = param;
        }

        public TObject GetParameter()
        {
            return m_Parameter;
        }

        private void _Initialize(TObject param)
        {
            SetParameter(param);
            m_Transitions = new Dictionary<int, List<TransitionBase<TObject>>>();
            m_States = new Dictionary<int, StateBase<TObject>>();
            m_States[StateMachineConst.ENTRY] = new StateBase<TObject>(StateMachineConst.ENTRY, this);
            m_States[StateMachineConst.END] = new StateBase<TObject>(StateMachineConst.END, this);
            Reset();
        }

        public StateBase<TObject> State<TState>(TState id) where TState : Enum
        {
            StateBase<TObject> state = new StateBase<TObject>(Convert.ToInt32(id), this);
            m_States[state.Id] = state;
            return state;
        }

        public StateMachine<TObject> Machine<TState>(TState id) where TState : Enum
        {
            StateMachine<TObject> machine = new StateMachine<TObject>(m_Parameter, Convert.ToInt32(id), this);
            m_States[machine.Id] = machine;
            return machine;
        }

        public StateMachine<TObject> SetDefault<TState>(TState defaultId) where TState : Enum
        {
            AddTransition(new TransitionBase<TObject>(StateMachineConst.ENTRY, Convert.ToInt32(defaultId), (so) => true, null, m_States[StateMachineConst.ENTRY]));
            return this;
        }

        public StateMachine<TObject> Build()
        {
            foreach (StateBase<TObject> state in m_States.Values)
                state.OnInitialize(m_Parameter);
            return this;
        }

        public StateMachine<TObject> Select<TState>(TState id, Func<TObject, bool> valid, TState toId, Action<TObject> transfer = null) where TState : Enum
        {
            int fromStateId = Convert.ToInt32(id);
            int toStateId = Convert.ToInt32(toId);
            foreach (int stateId in m_States.Keys)
                if (stateId == (fromStateId & stateId))
                    AddTransition(new TransitionBase<TObject>(stateId, toStateId, valid, transfer, m_States[stateId]));
            return this;
        }

        public StateMachine<TObject> Any<TState>(Func<TObject, bool> valid, TState toId, Action<TObject> transfer = null) where TState : Enum
        {
            int toStateId = Convert.ToInt32(toId);
            foreach (int stateId in m_States.Keys)
                if (stateId != StateMachineConst.ENTRY && stateId != StateMachineConst.END && stateId != toStateId)
                    AddTransition(new TransitionBase<TObject>(stateId, toStateId, valid, transfer, m_States[stateId]));
            return this;
        }

        internal override void OnExit(TObject param)
        {
            m_Current.OnExit(m_Parameter);
            Reset();
            base.OnExit(param);
        }

        internal override void OnUpdate(TObject param)
        {
            base.OnUpdate(param);
            Update();
        }
        public int GetCurrentId()
        {
            return m_Current == null ? StateMachineConst.END : m_Current.Id;
        }

        private void ProcessStateTransition(TransitionBase<TObject> transition)
        {
            m_Current.OnExit(m_Parameter);
            int targetStateId = transition.ToId == -1 ? m_Current.PreviousId : transition.ToId;
            if (!m_States.TryGetValue(targetStateId, out StateBase<TObject> nextState))
                throw new InvalidOperationException($"State {targetStateId} not defined.");
            int previousStateId = m_Current.Id;
            m_Current = nextState;
            nextState.PreviousId = previousStateId;
            transition.OnTransfer(m_Parameter);
            nextState.OnEnter(m_Parameter);
        }

        private bool IsValidStateForTransition(out List<TransitionBase<TObject>> transitions)
        {
            transitions = null;
            return !(m_Current == null || m_Current.Id == StateMachineConst.END || !m_Transitions.TryGetValue(m_Current.Id, out transitions));
        }

        public void Dispatch(FsmEvent fsmEvent)
        {
            if (IsValidStateForTransition(out List<TransitionBase<TObject>> transitions))
            {
                foreach (TransitionBase<TObject> transition in transitions)
                {
                    if (transition.EventType == fsmEvent.EventType && transition.OnEvent(m_Parameter, fsmEvent))
                    {
                        ProcessStateTransition(transition);
                        return;
                    }
                }

                foreach (StateBase<TObject> state in m_States.Values)
                    if(state is StateMachine<TObject>)
                        ((StateMachine<TObject>)state).Dispatch(fsmEvent);
            }
        }

        public void Update()
        {
            if (IsValidStateForTransition(out List<TransitionBase<TObject>> transitions))
            {
                m_Current.OnEarlyUpdate(m_Parameter);
                foreach (TransitionBase<TObject> transition in transitions)
                {
                    if (transition.OnValidate(m_Parameter))
                    {
                        ProcessStateTransition(transition);
                        return;
                    }
                }
                m_Current.OnUpdate(m_Parameter);
            }
        }

        public override void Dispose()
        {
            foreach (List<TransitionBase<TObject>> transitions in m_Transitions.Values)
                foreach (TransitionBase<TObject> transition in transitions)
                    transition.Dispose();
            foreach (StateBase<TObject> state in m_States.Values)
                state.Dispose();
            m_Transitions.Clear();
            m_Transitions = null;
            m_States.Clear();
            m_States = null;
            base.Dispose();
        }
    }
}
