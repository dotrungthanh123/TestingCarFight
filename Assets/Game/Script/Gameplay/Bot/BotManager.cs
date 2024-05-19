using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : Singleton<BotManager>
{
    [SerializeField] private int timeToActiveBot;

    private Bot[] bots;
    private int currentBot;
    private CounterTime counter;

    private void Awake() {
        currentBot = 0;
        bots = FindObjectsOfType<Bot>();
        counter = new CounterTime();

        for (int i = 0; i < bots.Length; i++) {
            bots[i].enabled = false;
        }
    }

    public void OnPlay() {
        for (int i = 0; i < bots.Length; i++) {
            bots[i].Reset();
        }

        counter.Start(ActiveBot, timeToActiveBot);
    }

    private void Update() {
        counter.Execute();
    }
    
    public void OnEnd() {
        for (int i = 0; i < bots.Length; i++) {
            bots[i].enabled = false;
        }

        counter.Cancel();
    }

    private void ActiveBot()
    {
        if (currentBot < bots.Length)
        {
            bots[currentBot].enabled = true;
            currentBot++;
        }
        
        counter.Start(ActiveBot, timeToActiveBot);
    }
}
