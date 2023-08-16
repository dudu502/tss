using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace Task.Switch.Structure.FSM
{
    public class StackState
    {
        public const byte STATEMACHINE_TYPE = 1;
        public const byte STATE_TYPE = 2;
        public const byte TRANSITION_TYPE = 3;
        
        public byte Type { private set; get; }
        public object Raw { private set; get; }

        public StackState(byte type, object raw)
        {
            Type = type;
            Raw = raw;
        }
        public T RawAs<T>()
        {
            return (T)Raw;
        }
    }
    public interface IStateMachine<TObject>
    {
        IState<TObject> State<TState>(TState id);
        IStateMachine<TObject> SetDefault<TState>(TState id);
        void Update();
        IStateMachine<TObject> Build();
        IStateMachine<TObject> Any<TState>(Func<TObject, bool> valid, TState toId, Action<TObject> transfer);
        IStateMachine<TObject> Select<TState>(Func<TObject,bool> valid, TState id,TState toId, Action<TObject> transfer);
    }
    public interface IState<TObject>
    {
        IState<TObject> Initialize(Action<TObject> onInit);
        IState<TObject> Enter(Action<TObject> onEnter);
        IState<TObject> Update(Action<TObject> onUpdate);
        IState<TObject> Exit(Action<TObject> onExit);
        IStateMachine<TObject> End();
        ITransition<TObject> Transition(Func<TObject, bool> valid);
    }
    public interface ITransition<TObject> 
    {
        ITransition<TObject> Transfer(Action<TObject> onTransfer);
        ITransition<TObject> To<TState>(TState id);
        IState<TObject> End();
    }

    internal class Transition<TObject>
    {
        internal int Id;
        internal int ToId;
        private Func<TObject, bool> m_Validate;
        private Action<TObject> m_Transfer;

        public Transition(int id, int toId, Func<TObject, bool> valid, Action<TObject> transfer)
        {
            Id = id;
            ToId = toId;
            m_Validate = valid;
            m_Transfer = transfer;
        }

        public Transition(int id,Func<TObject,bool> valid)
        {
            Id = id;
            m_Validate = valid;
        }
        
        public void SetTransfer(Action<TObject> transfer)
        {
            m_Transfer = transfer;
        }

        public Action<TObject> GetTransfer()
        {
            return m_Transfer;
        }

        public void OnTransfer(TObject param)
        {
            if (m_Transfer != null)
                m_Transfer(param);
        }

        public bool OnValidate(TObject param)
        {
            bool validate = false;
            if (m_Validate != null)
                validate = m_Validate(param);
            return validate;
        }
    }

    internal class State<TObject>
    {
        internal readonly int Id;
        public Action<TObject> OnInitialize;
        public Action<TObject> OnEnter;
        public Action<TObject> OnUpdate;
        public Action<TObject> OnExit;
        public State(int id)
        {
            Id = id;
        }
        public bool IsEntry()
        {
            return Id == int.MaxValue;
        }
        public bool IsEnd()
        {
            return Id == int.MinValue;
        }
    }

    public class StateMachine <TObject>: IStateMachine<TObject>,IState<TObject> , ITransition<TObject> where TObject : class
    {
        private State<TObject> m_Current;
        private readonly TObject m_Parameter;
        private readonly Dictionary<int, State<TObject>> m_States;
        private readonly Dictionary<int, List<Transition<TObject>>> m_Transitions;
        private Stack<StackState> m_StackBuilder = new Stack<StackState>();
        public StateMachine(TObject param)
        {
            m_States = new Dictionary<int, State<TObject>>();
            m_Transitions = new Dictionary<int, List<Transition<TObject>>>();
            m_Parameter = param;

            m_Current = AddState(int.MaxValue);
            AddState(int.MinValue);
            m_StackBuilder.Push(new StackState(StackState.STATEMACHINE_TYPE, this));
        }

        public IStateMachine<TObject> SetDefault<TState>(TState id) 
        {
            AddTransition(int.MaxValue, Convert.ToInt32(id), so => true, null);
            return this;
        }

        public IStateMachine<TObject> Build()
        {
            foreach (var state in m_States.Values)
                if (state.OnInitialize != null)
                    state.OnInitialize(m_Parameter);
            return this;
        }

        private State<TObject> AddState(int id)
        {
            State<TObject> state = new State<TObject>(id);
            return AddState(state);
        }

        private State<TObject> AddState(State<TObject> state)
        {
            m_States[state.Id] = state;
            return state;
        }

        private void AddTransition(int fromId, int toId, Func<TObject, bool> valid, Action<TObject> transfer)
        {
            Transition<TObject> transition = new Transition<TObject>(fromId, toId, valid, transfer);
            AddTransition(transition);
        }

        private void AddTransition(Transition<TObject> transition)
        {
            if (!m_Transitions.ContainsKey(transition.Id))
                m_Transitions[transition.Id] = new List<Transition<TObject>>();
            m_Transitions[transition.Id].Add(transition);
        }

        public IState<TObject> State<TState>(TState id) 
        {
            m_StackBuilder.Push(new StackState(StackState.STATE_TYPE,AddState(Convert.ToInt32(id))));
            return this;
        }

        IState<TObject> IState<TObject>.Initialize(Action<TObject> onInit)
        {
            StackState state = m_StackBuilder.Peek();
            if (state.Type == StackState.STATE_TYPE)
                state.RawAs<State<TObject>>().OnInitialize = onInit;
            return this;
        }
        IState<TObject> IState<TObject>.Enter(Action<TObject> onEnter)
        {
            StackState state = m_StackBuilder.Peek();
            if (state.Type == StackState.STATE_TYPE)
                state.RawAs<State<TObject>>().OnEnter = onEnter;
            return this;
        }
        IState<TObject> IState<TObject>.Update(Action<TObject> onUpdate)
        {
            StackState state = m_StackBuilder.Peek();
            if (state.Type == StackState.STATE_TYPE)
                state.RawAs<State<TObject>>().OnUpdate = onUpdate;
            return this;
        }
        IState<TObject> IState<TObject>.Exit(Action<TObject> onExit)
        {
            StackState state = m_StackBuilder.Peek();
            if (state.Type == StackState.STATE_TYPE)
                state.RawAs<State<TObject>>().OnExit = onExit;
            return this;
        }

        ITransition<TObject> IState<TObject>.Transition(Func<TObject, bool> valid)
        {
            StackState state = m_StackBuilder.Peek();
            if (state.Type == StackState.STATE_TYPE)
                m_StackBuilder.Push(new StackState(StackState.TRANSITION_TYPE, new Transition<TObject>(state.RawAs<State<TObject>>().Id, valid)));
            return this;
        }

        ITransition<TObject> ITransition<TObject>.Transfer(Action<TObject> onTransfer)
        {
            StackState state = m_StackBuilder.Peek();
            if (state.Type == StackState.TRANSITION_TYPE)
                state.RawAs<Transition<TObject>>().SetTransfer(onTransfer);
            return this;
        }

        ITransition<TObject> ITransition<TObject>.To<TState>(TState id)
        {
            StackState state = m_StackBuilder.Peek();
            if (state.Type == StackState.TRANSITION_TYPE)
                state.RawAs<Transition<TObject>>().ToId = Convert.ToInt32(id);
            return this;
        }

        IState<TObject> ITransition<TObject>.End()
        {
            StackState state = m_StackBuilder.Peek();
            if (state.Type == StackState.TRANSITION_TYPE)
                AddTransition(m_StackBuilder.Pop().RawAs<Transition<TObject>>());
            return this;
        }

        IStateMachine<TObject> IState<TObject>.End()
        {
            StackState state = m_StackBuilder.Peek();
            if (state.Type == StackState.STATE_TYPE)
                m_StackBuilder.Pop();
            return this;
        }

        IStateMachine<TObject> IStateMachine<TObject>.Select<TState>(Func<TObject, bool> valid, TState id, TState toId, Action<TObject> transfer = null)
        {
            int stateId = Convert.ToInt32(id);
            int toStateId = Convert.ToInt32(toId);
            foreach(var state in m_States.Values)
                if(state.Id == (stateId & state.Id))
                    AddTransition(state.Id, toStateId, valid, transfer);
            return this;
        }
        IStateMachine<TObject> IStateMachine<TObject>.Any<TState>(Func<TObject,bool> valid, TState toId, Action<TObject> transfer = null)
        {
            int toStateId = Convert.ToInt32(toId);
            foreach(var state in m_States.Values)
                if(state.Id != toStateId)
                    AddTransition(state.Id, toStateId, valid, transfer);
            return this;
        }

        void IStateMachine<TObject>.Update()
        {
            if (m_Current != null && !m_Current.IsEnd() && m_Transitions.ContainsKey(m_Current.Id))
            {
                foreach (Transition<TObject> transition in m_Transitions[m_Current.Id])
                {
                    if (transition.OnValidate(m_Parameter))
                    {
                        if (m_Current.OnExit != null)
                            m_Current.OnExit(m_Parameter);
                        m_Current = m_States[transition.ToId];
                        transition.OnTransfer(m_Parameter);
                        if (m_Current.OnEnter != null)
                            m_Current.OnEnter(m_Parameter);
                        return;
                    }
                }
                if(m_Current.OnUpdate != null)
                    m_Current.OnUpdate(m_Parameter);
            }
        }

    }
}
