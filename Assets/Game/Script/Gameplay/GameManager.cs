using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    private Player player;
    private Car[] cars;

    private void Awake() {
        player = FindObjectOfType<Player>();

        cars = FindObjectsOfType<Car>();
        for (int i = 0; i < cars.Length; i++) {
            cars[i].SetInput(0, 0);
        }
    }

    private void Start() {
        player.enabled = false;
    }

    public void OnPlay() {
        Clock.Ins.OnStart();
        player.Reset();
        player.enabled = true;
    }

    public void OnEnd() {
        for (int i = 0; i < cars.Length; i++) {
            cars[i].SetInput(0, 0);
        }
    }
    
}
