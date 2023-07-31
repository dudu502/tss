using System;
using System.Collections.Generic;

namespace Task.Switch.Structure.HFSM
{  
    public interface IStateDeclarable<TStateObject>
    {
        IStateDeclarable<TStateObject> Initialize(Action<TStateObject> init);
        IStateDeclarable<TStateObject> Enter(Action<TStateObject> enter);
        IStateDeclarable<TStateObject> Update(Action<TStateObject> update);
        IStateDeclarable<TStateObject> Exit(Action<TStateObject> exit);
        ITransitionDeclarable<TStateObject> When(Func<TStateObject, bool> valid);
    }
    public interface IStateMachineDeclarable<TStateObject>: IStateDeclarable<TStateObject>
    {
        StateMachineBuilder<TStateObject> Builder { get; }
        IStateMachineDeclarable<TStateObject> SetDefault(int state);
        StateMachineBuilder<TStateObject> Build();

    }
    public interface ITransitionDeclarable<TStateObject>
    {
        StateMachineBuilder<TStateObject> To(int state);
    }

    public class StateMachineBuilder<TStateObject>:IStateDeclarable<TStateObject>,
        ITransitionDeclarable<TStateObject>,
        IStateMachineDeclarable<TStateObject>
    {
        private Stack<StateMachineBuilder<TStateObject>> m_BuilderStack;
        private Stack<object> m_DeclarativeStack;
        private StateMachine<TStateObject> m_Machine;
        public StateMachineBuilder(StateMachine<TStateObject> machine)
        {
            m_DeclarativeStack = new Stack<object>();
            m_Machine = machine;

            m_BuilderStack = new Stack<StateMachineBuilder<TStateObject>>();
            m_BuilderStack.Push(this);
        }

        public IStateDeclarable<TStateObject> NewState(int id)
        {
            State<TStateObject> state = new State<TStateObject>(id);
            m_Machine.AddState(state);
            m_DeclarativeStack.Push(state);
            return this;
        }

        public IStateMachineDeclarable<TStateObject> NewStateMachine(int id)
        {
            StateMachine<TStateObject> machine = new StateMachine<TStateObject>(id);
            m_Machine.AddState(machine);
            m_DeclarativeStack.Push(machine);
            return this;
        }
        public StateMachineBuilder<TStateObject> Builder
        {
            get
            {
                StateMachine<TStateObject> stateMachine =  m_DeclarativeStack.Peek() as StateMachine<TStateObject>;
                if (stateMachine != null)
                {
                    StateMachineBuilder<TStateObject> builder = stateMachine.Builder;
                    builder.m_BuilderStack = m_BuilderStack;
                    builder.m_BuilderStack.Push(this);
                    return builder;
                }                  
                return this;
            }
        }

        public StateMachineBuilder<TStateObject> Build()
        {
            m_Machine.OnInitialize(m_Machine.GetStateObject());   
            return m_BuilderStack.Pop();
        }

        public IStateMachineDeclarable<TStateObject> SetDefault(int id)
        {
            m_Machine.SetDefault(id);
            return this;
        }

        public IStateDeclarable<TStateObject> Initialize(Action<TStateObject> init)
        {
            if(m_DeclarativeStack.TryPeek(out var state))
                ((State<TStateObject>)state).Initialize(init);
            return this;
        }

        public IStateDeclarable<TStateObject> Enter(Action<TStateObject> enter)
        {
            if (m_DeclarativeStack.TryPeek(out var state))
                ((State<TStateObject>)state).Enter(enter);
            return this;
        }

        public IStateDeclarable<TStateObject> Update(Action<TStateObject> update)
        {
            if (m_DeclarativeStack.TryPeek(out var state))
                ((State<TStateObject>)state).Update(update);
            return this;
        }

        public IStateDeclarable<TStateObject> Exit(Action<TStateObject> exit)
        {
            if (m_DeclarativeStack.TryPeek(out var state))
                ((State<TStateObject>)state).Exit(exit);
            return this;
        }

        public ITransitionDeclarable<TStateObject> When(Func<TStateObject,bool> valid)
        {
            if(m_DeclarativeStack.TryPeek(out var state))
            {
                Transition<TStateObject> transition = new Transition<TStateObject>(((State<TStateObject>)state).Id,int.MinValue,valid);
                m_Machine.AddTransition(transition);
                m_DeclarativeStack.Push(transition);
            }
            return this;
        }

        public StateMachineBuilder<TStateObject> Where(int fromId, Func<TStateObject, bool> valid)
        {
            foreach (var state in m_Machine.GetSubStates().Values)
            {
                if ((fromId & state.Id) == state.Id)
                {
                    Transition<TStateObject> transition = new Transition<TStateObject>(state.Id, int.MinValue, valid);
                    m_Machine.AddTransition(transition);
                    m_DeclarativeStack.Push(transition);
                }
            }
            return this;
        }

        public StateMachineBuilder<TStateObject> To(int id)
        {
            while (m_DeclarativeStack.TryPeek(out var transition) && transition.GetType() == typeof(Transition<TStateObject>))
            {
                if (id != ((Transition<TStateObject>)transition).Id)
                    ((Transition<TStateObject>)transition).ToId = id;
                m_DeclarativeStack.Pop();
            }
            return this;
        }

        public StateMachineBuilder<TStateObject> End()
        {
            m_DeclarativeStack.Pop();
            return this;
        }
    }
}
