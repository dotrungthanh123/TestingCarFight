using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckPoints : MonoBehaviour
{

    public class TrackEventArgs : EventArgs {
        public TrackEventArgs(Transform car) {
            this.car = car;
        }

        public Transform car;
    }

    private List<int> nextCheckpointIndex;
    public List<Transform> cars;

    public delegate void TrackEventHandler(object sender, TrackEventArgs e);
    public event TrackEventHandler onCorrectCheckpoint;
    public event TrackEventHandler onWrongCheckpoint;
    public CheckPointSingle[] checkPoints;

    private void Awake() {
        foreach (CheckPointSingle checkpoint in checkPoints) {
            checkpoint.trackCheckPoints = this;
        }

        nextCheckpointIndex = new List<int>();

        foreach (Transform car in cars) {
            nextCheckpointIndex.Add(0);
        }
    }

    public void CarThroughCheckpoint(CheckPointSingle checkPointSingle, Transform car) {
        int nextCheckpointSingleIndex = nextCheckpointIndex[cars.IndexOf(car)];
        if (Array.IndexOf(checkPoints, checkPointSingle) == nextCheckpointSingleIndex) {
            nextCheckpointIndex[nextCheckpointSingleIndex] = (nextCheckpointIndex[nextCheckpointSingleIndex] + 1) % checkPoints.Length;
            onCorrectCheckpoint?.Invoke(this, new TrackEventArgs(car));
            Debug.Log(1);
        } else {
            onWrongCheckpoint?.Invoke(this, new TrackEventArgs(car));
            Debug.Log(2);
        }
    }

    public void Reset(Transform car) {
        nextCheckpointIndex[cars.IndexOf(car)] = 0;
    }

    public CheckPointSingle getNextCheckPoint(Transform car)
    {   
        return checkPoints[nextCheckpointIndex[cars.IndexOf(car)]];
    }
}
