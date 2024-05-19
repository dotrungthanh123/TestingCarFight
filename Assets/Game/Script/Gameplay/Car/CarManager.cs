using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : Singleton<CarManager>
{
    [SerializeField] private Player[] players;

    private void Awake() {
        players = FindObjectsOfType<Player>(true);
    }

    public Player ChangeCar(int index) {
        return players[index];
    }
}
