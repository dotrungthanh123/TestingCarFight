using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashSpawner : Singleton<CashSpawner>
{

    [SerializeField] private Transform spawnPosition;
    [SerializeField] private CashPool cashPool;
    [SerializeField] private Transform cashParent;
    [SerializeField] private int delayBetweenSpawn;
    [SerializeField] private int spawnNum;

    private CounterTime counter;
    private Transform[] spawnPositions;
    private Dictionary<Vector3, bool> availablePositions;
    
    private void Start() {
        spawnPositions = spawnPosition.GetComponentsInChildren<Transform>();

        availablePositions = new Dictionary<Vector3, bool>();
        foreach (Transform transform in spawnPositions) {
            availablePositions[transform.position] = true;
        }

        counter = new CounterTime();
    }

    private void Spawn()
    {
        int toSpawn = spawnNum;

        for (int j = 0; j < spawnPositions.Length; j++)
        {
            if (availablePositions[spawnPositions[j].position])
            {
                Cash cash = cashPool.Get();
                cash.transform.position = spawnPositions[j].position;
                availablePositions[spawnPositions[j].position] = false;
                cash.transform.parent = cashParent;
                toSpawn--;
            }
            if (toSpawn == 0) break;
        }

        counter.Start(Spawn, delayBetweenSpawn);
    }

    private void Update() {
        if (counter.IsRunning) {
            counter.Execute();
        }
    }

    public void updatePosition(Vector3 position) {
        availablePositions[position] = true;
    }

    public void OnPlay() {
        counter.Start(Spawn, delayBetweenSpawn);
    }

    public void OnEnd() {
        counter.Cancel();
    }

}
