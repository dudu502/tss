using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.FSM
{
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
        private Func<TObject, object, bool> m_Event;
        private StateBase<TObject> m_State;

        public TransitionBase(int id, int toId, Func<TObject, bool> valid, Action<TObject> transfer, StateBase<TObject> stateBase)
        {
            m_State = stateBase;
            Id = id;
            ToId = toId;
            m_Validate = valid;
            m_Transfer = transfer;
            m_Event = null;
            EventType = null;
        }

        public TransitionBase(int id, int toId, string type, Func<TObject, object, bool> onEvt, Action<TObject> transfer, StateBase<TObject> stateBase)
        {
            m_State = stateBase;
            m_Event = onEvt;
            m_Transfer = transfer;
            m_Validate = null;
            ToId = toId;
            Id = id;
            EventType = type;
        }

        internal bool OnValidate(TObject param)
        {
            bool result = false;
            if (m_Validate != null)
            {
                result = m_Validate(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnValidate == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnValidate))
                    StateMachineDebug.Log($"Transition:{Id}->{ToId} {nameof(OnValidate)} Validate:{result} Parameters:{param}");
            }
            return result;
        }

        internal void OnTransfer(TObject param)
        {
            if (m_Transfer != null)
            {
                m_Transfer(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnTransfer == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnTransfer))
                    StateMachineDebug.Log($"Transition:{Id}->{ToId} {nameof(OnTransfer)} Parameters:{param}");
            }
        }

        internal bool OnEvent(TObject param, object evt)
        {
            bool result = false;
            if (m_Event != null)
            {
                result = m_Event(param, evt);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnEvent == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnEvent))
                    StateMachineDebug.Log($"Event:{Id}->{ToId} {nameof(OnEvent)} Event:{result} Parameters:{param} EventType:{EventType} EventData:{evt ?? "<null>"}");
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
            m_Event = null;
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
                    StateMachineDebug.Log($"StateId:{Id} {nameof(OnEnter)} Parameters:{param}");
            }
        }
        internal virtual void OnEarlyUpdate(TObject param)
        {
            if (m_OnEarlyUpdate != null)
            {
                m_OnEarlyUpdate(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnEarlyUpdate == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnEarlyUpdate))
                    StateMachineDebug.Log($"StateId:{Id} {nameof(OnEarlyUpdate)} Parameters:{param}");
            }
        }
        internal virtual void OnUpdate(TObject param)
        {
            if (m_OnUpdate != null)
            {
                m_OnUpdate(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnUpdate == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnUpdate))
                    StateMachineDebug.Log($"StateId:{Id} {nameof(OnUpdate)} Parameters:{param}");
            }
        }
        internal virtual void OnExit(TObject param)
        {
            if (m_OnExit != null)
            {
                m_OnExit(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnExit == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnExit))
                    StateMachineDebug.Log($"StateId:{Id} {nameof(OnExit)} Parameters:{param}");
            }
        }
        internal virtual void OnInitialize(TObject param)
        {
            if (m_OnInitialize != null)
            {
                m_OnInitialize(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnInitialize == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnInitialize))
                    StateMachineDebug.Log($"StateId:{Id} {nameof(OnInitialize)} Parameters:{param}");
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

        public TransitionBase<TObject> Event(string type, Func<TObject, object, bool> onEvt)
        {
            TransitionBase<TObject> transition = new TransitionBase<TObject>(Id, 0, type, onEvt, null, this);
            m_Parent.AddTransition(transition);
            return transition;
        }

        public virtual void Dispatch(string evtType,object evt)
        {
            return;
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

        public StateMachine<TObject> Select<TState>(TState id, string evtType, Func<TObject, object, bool> onEvt, TState toId, Action<TObject> transfer = null) where TState : Enum
        {
            int fromStateId = Convert.ToInt32(id);
            int toStateId = Convert.ToInt32(toId);
            foreach (int stateId in m_States.Keys)
                if (stateId == (fromStateId & stateId))
                    AddTransition(new TransitionBase<TObject>(stateId, toStateId, evtType, onEvt, transfer, m_States[stateId]));
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

        public StateMachine<TObject> Any<TState>(string evtType, Func<TObject, object, bool> onEvt, TState toId, Action<TObject> transfer = null) where TState : Enum
        {
            int toStateId = Convert.ToInt32(toId);
            foreach (int stateId in m_States.Keys)
                if (stateId != StateMachineConst.ENTRY && stateId != StateMachineConst.END && stateId != toStateId)
                    AddTransition(new TransitionBase<TObject>(stateId, toStateId, evtType, onEvt, transfer, m_States[stateId]));
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
            return m_Current.Id;
        }

        private void ProcessStateTransition(TransitionBase<TObject> transition)
        {
            m_Current.OnExit(m_Parameter);
            int targetStateId = transition.ToId == -1 ? m_Current.PreviousId : transition.ToId;
            if (!m_States.TryGetValue(targetStateId, out StateBase<TObject> nextState))
                throw new InvalidOperationException($"State {targetStateId} not defined.");
            int previousStateId = m_Current.Id;
            m_Current = nextState;
            m_Current.PreviousId = previousStateId;
            transition.OnTransfer(m_Parameter);
            m_Current.OnEnter(m_Parameter);
        }

        private bool IsValidStateForTransition(out List<TransitionBase<TObject>> transitions)
        {
            transitions = null;
            return m_Current.Id != StateMachineConst.END && m_Transitions.TryGetValue(m_Current.Id, out transitions);
        }
        
        public void Dispatch(string evtType)
        {
            Dispatch(evtType, null);
        }

        public override void Dispatch(string evtType, object evt)
        {
            if (IsValidStateForTransition(out List<TransitionBase<TObject>> transitions))
            {
                foreach (TransitionBase<TObject> transition in transitions)
                {
                    if (transition.EventType == evtType && transition.OnEvent(m_Parameter, evt))
                    {
                        ProcessStateTransition(transition);
                        return;
                    }
                }
                m_Current.Dispatch(evtType,evt);
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
