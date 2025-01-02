using System;
using System.Collections.Generic;
using Task.Switch.Structure.FSM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MyStateObject;


public class MyStateObject
{
    public enum GlobalState
    {
        Default = 1,
        Day_State = 2,
        Night_State = 3
    }
    public enum DayState
    {
        Eating = 1,
        Working = 2,
        Talking = 3,
        Exciting = 4,
        Idle = 5,
    }
    public enum NightState
    {
        Sleeping = 1,
        Dreaming = 2,
    }

    public const int MAX_TIME = 60*60*24;

    public static float EATING_SECS = UnityEngine.Random.Range(0.5f,0.6f);
    public static float TALKING_SECS = UnityEngine.Random.Range(0.8f, 1.6f);
    public static float EXCITING_SECS = UnityEngine.Random.Range(0.2f, 0.9f);
    public static float WORKING_SECS = UnityEngine.Random.Range(2f, 3f);
    public static float SLEEPING_SECS = UnityEngine.Random.Range(2f, 3f);
    public static float DREAMING_SECS = UnityEngine.Random.Range(0.5f, 0.8f);

    public int timestamp = 0;
    public Player player;
    public Image background;

    public TMPro.TMP_Text infoText;
    public Timer timer;
    public MyStateObject(Player player, Image background,TMP_Text infoText)
    {
        timer = new Timer();
        this.player = player;
        this.background = background;
        this.infoText = infoText;
    }
    public void TimePass()
    {
        timestamp += 20;
        timestamp %= MAX_TIME; 
        infoText.text = TimeString();
    }

    public void ShowPlayerInfo(string value,float progress)
    {
        player.SetInfo(value, 1-progress);
    }

    public string TimeString()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timestamp);
        return $"{timeSpan.Hours}:{timeSpan.Minutes}";
    }
    public bool IsDay()
    {
        return timestamp > (1 / 4f * MAX_TIME) && timestamp <= (3 / 4f * MAX_TIME);
    }
    public bool IsNight()
    {
        return (timestamp >= 0 && timestamp <= (1 / 4f * MAX_TIME)) || timestamp>(3/4f*MAX_TIME);
    }

    public override string ToString()
    {
        return $"[Time:{TimeString()} IsDay:{IsDay()} IsNight:{IsNight()}]";
    }
}


public class MainScene : MonoBehaviour
{
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;

    StateMachine<MyStateObject> stateMachine0;
    [SerializeField] Player player;
    [SerializeField] Image background;
    [SerializeField] TMP_Text infoText;
    MyStateObject stateObject0;

    void Start()
    {
        StateMachineDebug.Log = Debug.Log;
        StateMachineDebug.Filter = StateMachineDebug.LogFilter.Everything;
        stateObject0 = new MyStateObject(player, background, infoText);
        stateMachine0 = new StateMachine<MyStateObject>(stateObject0)
        #region DEFAULT STATE
            .State(GlobalState.Default)
                .Initialize(so => { })
                .Enter(so => { })
                .Transition(so => so.IsDay()).To(GlobalState.Day_State).End()
                .Transition(so => so.IsNight()).To(GlobalState.Night_State).End()
                .Exit(so => { })
            .End()
        #endregion
        #region DAY STATE
            .Machine(GlobalState.Day_State)
        #region SUB IDLE STATE
                .State(DayState.Idle)
                    .Initialize(so => { })
                    .Enter(so => { })
                    .Update(so => so.ShowPlayerInfo("idle", 1))
                    .Exit(so => { })
                    .Transition(so => true).To(DayState.Eating).End()
                .End()
        #endregion
        #region SUB EATING STATE
                .State(DayState.Eating)
                    .Initialize(so => { })
                    .Enter(so => so.timer.Reset())
                    .Update(so => so.ShowPlayerInfo("eating", so.timer / MyStateObject.EATING_SECS))
                    .Exit(so => { })
                    .Transition(so => so.timer > MyStateObject.EATING_SECS).To(DayState.Talking).End()
                .End()
        #endregion
        #region SUB TALKING STATE
                .State(DayState.Talking)
                    .Initialize(so => { })
                    .Enter(so => so.timer.Reset())
                    .Update(so => so.ShowPlayerInfo("talking", so.timer / MyStateObject.TALKING_SECS))
                    .Exit(so => { })
                    .Transition(so => so.timer > MyStateObject.TALKING_SECS).To(DayState.Working).End()
                .End()
        #endregion
        #region SUB WORKING STATE
                .State(DayState.Working)
                    .Initialize(so => { })
                    .Enter(so => so.timer.Reset())
                    .Update(so => so.ShowPlayerInfo("working", so.timer / MyStateObject.WORKING_SECS))
                    .Exit(so => { })
                    .Transition(so => so.timer > MyStateObject.WORKING_SECS).To(DayState.Exciting).End()
                .End()
        #endregion
        #region SUB EXCITING STATE
                .State(DayState.Exciting)
                    .Initialize(so => { })
                    .Enter(so => so.timer.Reset())
                    .Update(so => so.ShowPlayerInfo("exciting", so.timer / MyStateObject.EXCITING_SECS))
                    .Exit(so => { })
                    .Transition(so => so.timer > MyStateObject.EXCITING_SECS).To(DayState.Idle).End()
                .End()
        #endregion
                .Build().SetDefault(DayState.Idle)
                .Initialize(so => { })
                .EarlyUpdate(so => { })
                .Enter(so => so.background.color = dayColor)
                .Update(so => so.TimePass())
                .Transition(so => so.IsNight()).To(GlobalState.Night_State).End()
                .Exit(so => { })
            .End()
        #endregion
        #region NIGHT STATE
            .Machine(GlobalState.Night_State)
        #region SUB SLEEPING STATE
                .State(NightState.Sleeping)
                    .Initialize(so => { })
                    .Enter(so => so.timer.Reset())
                    .Update(so => so.ShowPlayerInfo("sleeping", so.timer / MyStateObject.SLEEPING_SECS))
                    .Exit(so => { })
                    .Transition(so => so.timer > MyStateObject.SLEEPING_SECS).To(NightState.Dreaming).End()
                .End()
        #endregion
        #region SUB DREAMING STATE
                .State(NightState.Dreaming)
                    .Initialize(so => { })
                    .Enter(so => so.timer.Reset())
                    .Update(so => so.ShowPlayerInfo("dreaming", so.timer / MyStateObject.DREAMING_SECS))
                    .Exit(so => { })
                    .Transition(so => so.timer > MyStateObject.DREAMING_SECS).To(NightState.Sleeping).End()
                .End()
        #endregion
                .Build().SetDefault(NightState.Sleeping)

                .Initialize(so => { })
                .Enter(so => so.background.color = nightColor)
                .Update(so => so.TimePass())
                .Exit(so => { })
                .Transition(so => so.IsDay()).To(GlobalState.Day_State).End()
            .End()
        #endregion
            .Build().SetDefault(GlobalState.Default);
    }

    void Update()
    {
        stateMachine0.Update();
    }
}
