using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFollow : MonoBehaviour
{
    [SerializeField] private Car target;

    [Header("Sensor")]
    [SerializeField] Transform leftSensor;
    [SerializeField] Transform rightSensor;
    [SerializeField] Transform midSensor;
    [SerializeField] float sensorAngle;
    [SerializeField] float sensorAngleStep;
    [SerializeField] float sensorDistance;
    [SerializeField] float verAvoidInput;

    private Car car;
    private float horInput, verInput;

    private void Start() {
        car = GetComponent<Car>();   
    }

    public void SetTarget(Car car) {
        target = car;
    }

    private void Update() {
        // Transform target world position to bot local position
        Vector3 targetLocalPosition = transform.InverseTransformPoint(
            target.transform.position.x,
            transform.position.y,
            target.transform.position.z);

        // The car move forward on z axis and steer on x axis
        // Use the x value on the magnitude gives the direction to turn left or right
        // Equation does not specifically calculating how much to steer but more on steering left or right
        float steerDir = targetLocalPosition.x / targetLocalPosition.magnitude;
        float targetRotationAngle = car.maxSteeringAngle * steerDir;
        if (targetRotationAngle > car.CurrentSteeringAngle)
        {
            horInput = 1;
        } else if (targetRotationAngle > car.CurrentSteeringAngle)
        {
            horInput = -1;
        } else {
            horInput = 0;
        }

        Sensor();
    }

    private void Sensor() {
        verAvoidInput = 0;
        // Angle use to calculate how much to increase verAvoidInput
        float baseAngle = 180;

        int numOfRay = Mathf.FloorToInt(sensorAngle / sensorAngleStep);

        for (int i = 0; i < numOfRay; i++) {
            // origion - direction - hit - distance - layer

            // left
            Vector3 direction = Quaternion.AngleAxis(-i * sensorAngleStep, transform.up) * transform.forward;
            if (Physics.Raycast(leftSensor.position, direction, out RaycastHit hit, sensorDistance)) {
                // Angle A = angle between normal and direction
                // |A| proportional to angle required to rotate

                float A = Mathf.Abs(Vector3.Angle(hit.normal, direction));
            }
        }
    }
}
