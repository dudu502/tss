using System;
using System.Collections;
using System.Collections.Generic;
using Task.Switch.Structure.HFSM;
using UnityEngine;
using UnityEngine.UI;

public class StateObject
{
    public const int DEFAULT = 10;
    public const int DAY_STATE = 1;
    public const int NIGHT_STATE = 2;

    public const int EATING = 3;
    public const int WORKING = 4;
    public const int TALKING = 5;

    public const int EXCITING = 6;
    public const int IDLE = 7;

    public const int SLEEPING = 8;
    public const int DREAMING = 9;

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
    public StateObject(Player player,Image background, TMPro.TMP_Text infoText)
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
}

public class MainScene : MonoBehaviour
{
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;
    [SerializeField] Player player;
    [SerializeField] Image backgroundImage;
    [SerializeField] TMPro.TMP_Text infoText;
    StateMachine<StateObject> stateMachine;

    void Start()
    {
        //StateMachineLogger.LogInfo = Debug.LogError;
        stateMachine = new StateMachine<StateObject>(new StateObject(player, backgroundImage, infoText));
        stateMachine
            .Builder
                .NewState(StateObject.DEFAULT)
                    .Initialize(so => { })
                    .When(so => so.IsDay()).To(StateObject.DAY_STATE)
                    .When(so => so.IsNight()).To(StateObject.NIGHT_STATE)
                .End()
                .NewStateMachine(StateObject.DAY_STATE)
                    .Initialize(so => { })
                    .Enter(so => so.background.color = dayColor)
                    .Update(so => so.TimePass())
                    .When(so => so.IsNight()).To(StateObject.NIGHT_STATE)
                    .Builder
                        .NewState(StateObject.IDLE)
                            .Initialize(so => { })
                            .Update(so => so.ShowPlayerInfo("idle", 1))
                            .When(so => true).To(StateObject.EATING)
                        .End()
                        .NewState(StateObject.EATING)
                            .Initialize(so => { })
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("eating", so.timer / StateObject.EATING_SECS))
                            .When(so => so.timer > StateObject.EATING_SECS).To(StateObject.TALKING)
                        .End()
                        .NewState(StateObject.TALKING)
                            .Initialize(so => { })
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("talking", so.timer / StateObject.TALKING_SECS))
                            .When(so => so.timer > StateObject.TALKING_SECS).To(StateObject.WORKING)
                        .End()
                        .NewState(StateObject.WORKING)
                            .Initialize(so => { })
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("working", so.timer / StateObject.WORKING_SECS))
                            .When(so => so.timer > StateObject.WORKING_SECS).To(StateObject.EXCITING)
                        .End()
                        .NewState(StateObject.EXCITING)
                            .Initialize(so => { })
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("exciting", so.timer / StateObject.EXCITING_SECS))
                            .When(so => so.timer > StateObject.EXCITING_SECS).To(StateObject.IDLE)
                        .End()
                    .SetDefault(StateObject.IDLE).Build()
                .End()
                .NewStateMachine(StateObject.NIGHT_STATE)
                    .Initialize(so => { })
                    .Enter(so => so.background.color = nightColor)
                    .Update(so => so.TimePass())
                    .When(so => so.IsDay()).To(StateObject.DAY_STATE)
                    .Builder
                        .NewState(StateObject.SLEEPING)
                            .Initialize(so => { })
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("sleeping", so.timer / StateObject.SLEEPING_SECS))
                            .When(so => so.timer > StateObject.SLEEPING_SECS).To(StateObject.DREAMING)
                        .End()
                        .NewState(StateObject.DREAMING)
                            .Initialize(so => { })
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("dreaming", so.timer / StateObject.DREAMING_SECS))
                            .When(so => so.timer > StateObject.DREAMING_SECS).To(StateObject.SLEEPING)
                        .End()
                    .SetDefault(StateObject.SLEEPING).Build()
                .End()
                .SetDefault(StateObject.DEFAULT).Build();
    }

    void Update()
    {
        stateMachine.Update();
    }
}
