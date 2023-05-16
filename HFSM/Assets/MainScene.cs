using System;
using System.Collections;
using System.Collections.Generic;
using Task.Switch.Structure.HFSM;
using UnityEngine;
using UnityEngine.UI;

public class StateObject
{
    public const int DEFAULT = 0;
    public const int DAY_STATE = 1;
    public const int NIGHT_STATE = 2;

    public const int EATING = 1;
    public const int WORKING = 2;
    public const int TALKING = 3;

    public const int EXCITING = 4;
    public const int IDLE = 5;

    public const int SLEEPING = 1;
    public const int DREAMING = 2;

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
    }
    public void ShowTime()
    {
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
        stateMachine = new StateMachine<StateObject>(new StateObject(player, backgroundImage, infoText));
        stateMachine
            .Builder
                .NewStateMachine(StateObject.DEFAULT)
                    .Translate(so => so.IsDay()).To(StateObject.DAY_STATE)
                    .Translate(so => so.IsNight()).To(StateObject.NIGHT_STATE)
                .End()
                .NewStateMachine(StateObject.DAY_STATE)
                    .Enter(so => so.background.color = dayColor)
                    .Update(so =>
                    {
                        so.TimePass();
                        so.ShowTime();
                    })
                    .Translate(so => so.IsNight()).To(StateObject.NIGHT_STATE)
                    .Builder
                        .NewState(StateObject.IDLE)
                            .Update(so => so.ShowPlayerInfo("idle", 1))
                            .Translate(so => true).To(StateObject.EATING)
                        .End()
                        .NewState(StateObject.EATING)
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("eating", so.timer / StateObject.EATING_SECS))
                            .Translate(so => so.timer > StateObject.EATING_SECS).To(StateObject.TALKING)
                        .End()
                        .NewState(StateObject.TALKING)
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("talking", so.timer / StateObject.TALKING_SECS))
                            .Translate(so => so.timer > StateObject.TALKING_SECS).To(StateObject.WORKING)
                        .End()
                        .NewState(StateObject.WORKING)
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("working", so.timer / StateObject.WORKING_SECS))
                            .Translate(so => so.timer > StateObject.WORKING_SECS).To(StateObject.EXCITING)
                        .End()
                        .NewState(StateObject.EXCITING)
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("exciting", so.timer / StateObject.EXCITING_SECS))
                            .Translate(so => so.timer > StateObject.EXCITING_SECS).To(StateObject.IDLE)
                        .End()
                    .SetDefault(StateObject.IDLE).Build()
                .End()
                .NewStateMachine(StateObject.NIGHT_STATE)
                    .Enter(so => so.background.color = nightColor)
                    .Update(so =>
                    {
                        so.TimePass();
                        so.ShowTime();
                    })
                    .Translate(so => so.IsDay()).To(StateObject.DAY_STATE)
                    .Builder
                        .NewState(StateObject.SLEEPING)
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("sleeping", so.timer / StateObject.SLEEPING_SECS))
                            .Translate(so => so.timer > StateObject.SLEEPING_SECS).To(StateObject.DREAMING)
                        .End()
                        .NewState(StateObject.DREAMING)
                            .Enter(so => so.timer.Reset())
                            .Update(so => so.ShowPlayerInfo("dreaming", so.timer / StateObject.DREAMING_SECS))
                            .Translate(so => so.timer > StateObject.DREAMING_SECS).To(StateObject.SLEEPING)
                        .End()
                    .SetDefault(StateObject.SLEEPING).Build()
                .End()
                .SetDefault(StateObject.DEFAULT).Build();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Tick();
    }

}
