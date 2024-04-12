using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointSingle : MonoBehaviour
{
    public TrackCheckPoints trackCheckPoints;

    private void OnTriggerEnter(Collider other) {
        trackCheckPoints.CarThroughCheckpoint(this, other.transform);
    }
}
