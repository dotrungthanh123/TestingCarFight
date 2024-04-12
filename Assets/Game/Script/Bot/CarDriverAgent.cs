using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarDriverAgent : Agent
{

    public TrackCheckPoints track;
    public Transform spawnPosition;

    private Car car;

    private void Awake() {
        car = GetComponent<Car>();
    }

    private void Start() {
        track.onCorrectCheckpoint += OnCorrectCheckpoint;
        track.onWrongCheckpoint += OnWrongCheckpoint;
    }

    private void OnCorrectCheckpoint(object sender, TrackCheckPoints.TrackEventArgs e) {
        if (e.car == transform) {
            AddReward(1);
        }
    }

    private void OnWrongCheckpoint(object sender, TrackCheckPoints.TrackEventArgs e) {
        if (e.car == transform) {
            AddReward(-1);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 checkPointForward = track.getNextCheckPoint(transform).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkPointForward);
        sensor.AddObservation(directionDot);
        sensor.AddObservation(car.CarRigidbody.velocity);
    }

    public override void OnEpisodeBegin()
    {
        transform.position = spawnPosition.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
        transform.forward = spawnPosition.forward;
        track.Reset(transform);
        car.Stop();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int verInput = 0, horInput = 0;

        switch (actions.DiscreteActions[0]) {
            case 0: verInput = 0; break;
            case 1: verInput = 1; break;
            case 2: verInput = -1; break;
        }

        switch (actions.DiscreteActions[1]) {
            case 0: horInput = 0; break;
            case 1: horInput = 1; break;
            case 2: horInput = -1; break;
        }

        car.SetInput(horInput, verInput);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = car.VerInput;
        discreteActions[1] = car.HorInput;
    }


}
