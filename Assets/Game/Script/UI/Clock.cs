using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using TMPro;
using UnityEngine;

public class Clock : Singleton<Clock>
{

    private ButtonManager timer;
    private float elapsedTime;
    private bool playing;

    public float ElapsedTime => elapsedTime; 

    private void Start() {
        timer = GetComponent<ButtonManager>();
        timer.isInteractable = false;

        elapsedTime = 0;
    }

    private void Update() {
        if (playing) {
            UpdateTime();
        }
    }

    private void UpdateTime() {
        elapsedTime += Time.deltaTime;
        string formattedTime = TimeSpan.FromSeconds(elapsedTime).ToString("mm\\:ss");
        timer.SetText(formattedTime);
    }
    
    public void OnStart() {
        playing = true;
        elapsedTime = 0;
    }

    public void OnEnd() {
        playing = false;
    }
}
