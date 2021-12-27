using System;
using System.Collections.Generic;
using System.Text;

namespace FSM
{
    public class State
    {
        public List<Translation> Translations { private set; get; }
        private Action m_OnInitialize;
        private Action m_OnEnter;
        private Action m_OnUpdate;
        private Action m_OnExit;
        public string Name { set; get; }

        public State()
        {
            Translations = new List<Translation>();
        }

        
        public Translation Translate(string translationName)
        {
            Translation translation = new Translation(this);
            translation.Name = translationName;
            Translations.Add(translation);
            return translation;
        }
        public State Initialize(Action init) { m_OnInitialize = init; return this; }
        public State Enter(Action enter) { m_OnEnter = enter; return this; }
        public State Update(Action update) { m_OnUpdate = update; return this; }
        public State Exit(Action exit) { m_OnExit = exit; return this; }
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
