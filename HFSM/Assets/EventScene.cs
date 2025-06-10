using Task.Switch.Structure.FSM;
using TMPro;
using UnityEngine;

public class EventScene : MonoBehaviour
{
    public enum StateType
    {
        Default,
        Damage,
        Heal
    }
    public class StateObj
    {
        public int counter = 50;
        public Timer timer = new Timer();
        public StateObj(int counter)
        {
            this.counter = counter;

        }
    }

    StateMachine<StateObj> machine;
    public TMP_Text text;
    private void Start()
    {
        StateMachineDebug.Filter = StateMachineDebug.LogFilter.OnUpdate | StateMachineDebug.LogFilter.OnEvent;
        StateMachineDebug.Log = Debug.Log;
        machine = new StateMachine<StateObj>(new StateObj(90))
            .State(StateType.Default)
                .Enter(so =>
                {

                })
                .Transition(so => so.counter < 10).To(StateType.Heal)
                .Transition(so => so.counter > 80).To(StateType.Damage)
                .Event("E2", (so, evt) => true).To(StateType.Heal)
                .Event("E3", (so, evt) => false).To(StateType.Heal)
            .End()
            .State(StateType.Damage)
                .Enter(so => so.timer.Reset())
                .Update(so =>
                {
                    if (so.timer > 0.5f)
                    {
                        so.timer.Reset();
                        so.counter--;
                    }
                })
                .Event("E1", (so, evt) => { return true; }).To(StateType.Heal)
                .Transition(so => so.counter < 55).To(StateType.Default)
            .End()
            .State(StateType.Heal)
                .Enter(so => so.timer.Reset())
                .Update(so =>
                {
                    if (so.timer > 0.5f)
                    {
                        so.timer.Reset();
                        so.counter++;
                    }

                })
                .Transition(so => so.counter >= 100).To(StateType.Default)
            .End()
            .Build().SetDefault(StateType.Default);
    }

    private void Update()
    {
        text.text = machine.GetParameter().counter.ToString();
        machine.Update();

        if (Input.GetKeyDown(KeyCode.E))
        {
            machine.Dispatch(new FsmEvent("E1", null));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            machine.Dispatch(new FsmEvent("E2", null));
        }
    }
}
