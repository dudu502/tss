using System;
using System.Collections.Generic;
using System.Text;

namespace FSM
{
    public class State<T> where T:Enum
    {
        public readonly List<Translation<T>> Translations = new List<Translation<T>>();
        private Action m_OnInitialize;
        private Action m_OnEnter;
        private Action m_OnUpdate;
        private Action m_OnExit;
        public T Name { set; get; }

        public State()
        {
            
        }

        public Translation<T> Translate()
        {
            return Translate(string.Empty);
        }
        public Translation<T> Translate(string translationName)
        {
            Translation<T> translation = new Translation<T>(this);
            translation.Name = translationName;
            Translations.Add(translation);
            return translation;
        }
        public State<T> Initialize(Action init) { m_OnInitialize = init; return this; }
        public State<T> Enter(Action enter) { m_OnEnter = enter; return this; }
        public State<T> Update(Action update) { m_OnUpdate = update; return this; }
        public State<T> Exit(Action exit) { m_OnExit = exit; return this; }
        public void OnInitialize()
        {
            m_OnInitialize?.Invoke();
        }
        public void OnEnter()
        {
            m_OnEnter?.Invoke();
        }
        public void OnUpdate()
        {
            m_OnUpdate?.Invoke();
        }
        public void OnExit()
        {
            m_OnExit?.Invoke();
        }
    }
}
