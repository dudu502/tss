using System;
using System.Collections.Generic;
using static Task.Switch.Structure.FSM.StateMachine;

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
            Everything = OnInitialize | OnEnter | OnEarlyUpdate | OnUpdate | OnExit | OnValidate | OnTransfer,
        }
        public static Action<string> Log;
        public static LogFilter Filter = LogFilter.Nothing;
    }

    public class TransitionBase
    {
        internal StateBase m_ParentState { get; set; }
        public int Id { get; private set; }
        public int ToId { get; set; }
        private Func<object, bool> m_Validate;
        private Action<object> m_Transfer;

        public TransitionBase(int id, int toId, Func<object, bool> valid, Action<object> transfer)
        {
            Id = id;
            ToId = toId;
            m_Validate = valid;
            m_Transfer = transfer;
        }
        internal bool OnValidate(object param)
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
        internal void OnTransfer(object param)
        {
            if (m_Transfer != null)
            {
                m_Transfer(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnTransfer == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnTransfer))
                    StateMachineDebug.Log($"<color=white>Transition:{Id}->{ToId} {nameof(OnTransfer)} Parameters:{param}</color>");
            }
        }
        public TransitionBase Transfer(Action<object> transfer)
        {
            m_Transfer = transfer;
            return this;
        }
        public TransitionBase To<TState>(TState toId) where TState : Enum
        {
            ToId = Convert.ToInt32(toId);
            return this;
        }

        public TransitionBase ToEnd()
        {
            ToId = StateMachine.END;
            return this;
        }

        public TransitionBase ToEntry()
        {
            ToId = StateMachine.ENTRY;
            return this;
        }

        public StateBase End()
        {
            return m_ParentState;
        }
    }

    public class StateBase
    {
        internal IStateMachine m_Parent;
        public int Id { get; private set; }
        private Action<object> m_OnInitialize;
        private Action<object> m_OnEnter;
        private Action<object> m_OnUpdate;
        private Action<object> m_OnExit;
        private Action<object> m_OnEarlyUpdate;

        public virtual StateBase Initialize(Action<object> init)
        {
            m_OnInitialize = init;
            return this;
        }

        public virtual StateBase Enter(Action<object> enter)
        {
            m_OnEnter = enter;
            return this;
        }
        public virtual StateBase Update(Action<object> update)
        {
            m_OnUpdate = update;
            return this;
        }
        public virtual StateBase EarlyUpdate(Action<object> earlyUpdate)
        {
            m_OnEarlyUpdate = earlyUpdate;
            return this;
        }
        public virtual StateBase Exit(Action<object> exit)
        {
            m_OnExit = exit;
            return this;
        }

        public virtual void OnEnter(object param)
        {
            if (m_OnEnter != null)
            {
                m_OnEnter(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnEnter == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnEnter))
                    StateMachineDebug.Log($"<color=magenta>StateId:{Id} {nameof(OnEnter)} Parameters:{param}</color>");
            }
        }
        public virtual void OnEarlyUpdate(object param)
        {
            if (m_OnEarlyUpdate != null)
            {
                m_OnEarlyUpdate(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnEarlyUpdate == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnEarlyUpdate))
                    StateMachineDebug.Log($"<color=#ff8000>StateId:{Id} {nameof(OnEarlyUpdate)} Parameters:{param}</color>");
            }
        }
        public virtual void OnUpdate<TObject>(TObject param)
        {
            if (m_OnUpdate != null)
            {
                m_OnUpdate(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnUpdate == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnUpdate))
                    StateMachineDebug.Log($"<color=yellow>StateId:{Id} {nameof(OnUpdate)} Parameters:{param}</color>");
            }
        }
        public virtual void OnExit(object param)
        {
            if (m_OnExit != null)
            {
                m_OnExit(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnExit == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnExit))
                    StateMachineDebug.Log($"<color=red>StateId:{Id} {nameof(OnExit)} Parameters:{param}</color>");
            }
        }
        public virtual void OnInitialize(object param)
        {
            if (m_OnInitialize != null)
            {
                m_OnInitialize(param);
                if (StateMachineDebug.Log != null && StateMachineDebug.LogFilter.OnInitialize == (StateMachineDebug.Filter & StateMachineDebug.LogFilter.OnInitialize))
                    StateMachineDebug.Log($"<color=green>StateId:{Id} {nameof(OnInitialize)} Parameters:{param}</color>");
            }
        }
        public StateBase(int id)
        {
            Id = id;
        }

        public TransitionBase Transition(Func<object, bool> valid)
        {
            TransitionBase transition = new TransitionBase(Id, 0, valid, null);
            transition.m_ParentState = this;
            m_Parent.AddTransition(transition);
            return transition;
        }
        public IStateMachine End()
        {
            return m_Parent;
        }

        public IStateMachine AsStateMachine()
        {
            if(!(this is IStateMachine))
                throw new Exception("Use the correct NewState() to create a StateMachine.");
            return this as IStateMachine;
        }
    }
    public interface IStateMachine
    {
        void Tick();
        StateBase NewState<TState>(TState id) where TState : Enum;
        StateMachine NewState<TState>(TState id, object param) where TState : Enum;
        StateMachine SetDefault<TState>(TState id) where TState : Enum;
        StateMachine Build();
        void AddState(StateBase state);
        void AddTransition(TransitionBase transition);

        StateMachine Select<TState>(Func<object,bool> valid,TState id,TState toId, Action<object> transfer) where TState : Enum;
        StateMachine Any<TState>(Func<object, bool> valid,TState toId, Action<object> transfer) where TState : Enum;
    }
    public class StateMachine : StateBase, IStateMachine
    {

        Dictionary<int, StateBase> m_States;
        Dictionary<int, List<TransitionBase>> m_Transitions;
        StateBase m_Current;
        object m_Parameter;


        public const int ENTRY = int.MaxValue - 1;
        public const int END = int.MaxValue;
        public StateMachine(object param, int id) : base(id)
        {
            _Initialize(param);
        }

        public StateMachine(object param) : base(int.MinValue)
        {
            _Initialize(param);
        }

        public void Reset()
        {
            m_Current = m_States[ENTRY];
        }

        public void AddState(StateBase state)
        {
            m_States[state.Id] = state;
        }
        public void AddTransition(TransitionBase transition)
        {
            if (!m_Transitions.ContainsKey(transition.Id))
                m_Transitions[transition.Id] = new List<TransitionBase>();
            m_Transitions[transition.Id].Add(transition);
        }
        private void _Initialize(object param)
        {
            m_Parameter = param;
            m_Transitions = new Dictionary<int, List<TransitionBase>>();
            m_States = new Dictionary<int, StateBase>();
            m_States[ENTRY] = new StateBase(ENTRY);
            m_States[ENTRY].m_Parent = this;
            m_States[END] = new StateBase(END);
            m_States[END].m_Parent = this;
            m_Current = m_States[ENTRY];
        }



        public StateBase NewState<TState>(TState id) where TState : Enum
        {
            StateBase state = new StateBase(Convert.ToInt32(id));
            m_States[state.Id] = state;
            state.m_Parent = this;
            return state;
        }

        public StateMachine NewState<TState>(TState id, object param) where TState : Enum
        {
            StateMachine machine = new StateMachine(param, Convert.ToInt32(id));
            m_States[machine.Id] = machine;
            machine.m_Parent = this;
            
            return machine;
        }

        public StateMachine SetDefault<TState>(TState defaultId) where TState : Enum
        {
            AddTransition(new TransitionBase(ENTRY, Convert.ToInt32(defaultId), (so) => true, null));
            return this;
        }
        public StateMachine Build()
        {
            foreach (StateBase state in m_States.Values)
            {
                state.OnInitialize(m_Parameter);
            }
            return this;
        }

        public StateMachine Select<TState>(Func<object, bool> valid, TState id, TState toId, Action<object> transfer) where TState : Enum
        {
            int fromStateId = Convert.ToInt32(id);
            int toStateId = Convert.ToInt32(toId);
            foreach(int stateId in m_States.Keys)
                if(stateId == (fromStateId & stateId))
                    AddTransition(new TransitionBase(stateId,toStateId,valid,transfer));
            return this;
        }
        public StateMachine Any<TState>(Func<object, bool> valid, TState toId, Action<object> transfer) where TState : Enum
        {
            int toStateId = Convert.ToInt32(toId);
            foreach (int stateId in m_States.Keys)
                if (stateId != toStateId)
                    AddTransition(new TransitionBase(stateId,toStateId,valid,transfer));
            return this;
        }
        public override void OnExit(object param)
        {
            m_Current.OnExit(m_Parameter);
            Reset();
            base.OnExit(param);
        }

        public void Tick()
        {
            if (m_Current != null && m_Current.Id != END && m_Transitions.TryGetValue(m_Current.Id, out List<TransitionBase> translations))
            {
                m_Current.OnEarlyUpdate(m_Parameter);
                foreach (TransitionBase transition in translations)
                {
                    if (transition.OnValidate(m_Parameter))
                    {
                        m_Current.OnExit(m_Parameter);

                        if (m_States.TryGetValue(transition.ToId, out StateBase next))
                        {
                            m_Current = next;
                            transition.OnTransfer(m_Parameter);
                            m_Current.OnEnter(m_Parameter);
                        }
                        else
                        {
                            throw new Exception("Use NewState() to define a State or a StateMachine.");
                        }
                        return;
                    }
                }
                m_Current.OnUpdate(m_Parameter);

                if (m_Current is IStateMachine)
                {
                    ((IStateMachine)m_Current).Tick();
                }
            }
        }
    }
}
