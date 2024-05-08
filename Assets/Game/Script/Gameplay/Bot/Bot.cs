using System;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private Car target;
    [Tooltip("Proportion of the car max speed")]
    [Range(0, 1)]
    [SerializeField] private float targetSpeed;

    [Header("Sensor")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform leftSensor;
    [SerializeField] private Transform rightSensor;
    [SerializeField] private Transform midSensor;
    [SerializeField] private float sensorAngle;
    [SerializeField] private float sensorAngleStep;
    [SerializeField] private float sensorDistance;
    [SerializeField] private float horAvoidScaler;
    [SerializeField] private float minAngleToSteer;

    [Header("Avoid Parameter")]
    [SerializeField] 
    // Angle use to calculate how much to increase horAvoidInput
    float baseAngle = 180;
    private float horAvoidInput;

    private Car car;
    private Rigidbody rb;
    private float horInput, verInput;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [Header("Debug")]
    private float currentSteeringAngle, steerDir, targetRotationAngle, velocity;

    private void Awake() {
        car = GetComponent<Car>();    
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void SetTarget(Car car) {
        target = car;
    }

    private void Update() {
        Sensor();

        horAvoidInput = Mathf.Clamp(horAvoidScaler * horAvoidInput, -1.75f, 1.75f);

        float targetSpeed = this.targetSpeed * car.CarTopSpeed / Mathf.Clamp(Mathf.Abs(horAvoidInput), 1 , 2);

        // Transform target world position to bot local position
        Vector3 targetLocalPosition = transform.InverseTransformPoint(
            target.transform.position.x,
            transform.position.y,
            target.transform.position.z);

        // The car move forward on z axis and steer on x axis
        // Use the x value on the magnitude gives the direction to turn left or right
        // Equation does not specifically calculating how much to steer but more on steering left or right

        currentSteeringAngle = car.CurrentSteeringAngle > car.MaxSteeringAngle + 1 ?
                                                            car.CurrentSteeringAngle - 360 : car.CurrentSteeringAngle;

        steerDir = targetLocalPosition.x / targetLocalPosition.magnitude;
        targetRotationAngle = car.MaxSteeringAngle * steerDir;

        if (Mathf.Abs(currentSteeringAngle - targetRotationAngle) > minAngleToSteer)
        {
            if (targetRotationAngle > currentSteeringAngle)
            {
                horInput = 1;
            }
            else if (targetRotationAngle < currentSteeringAngle)
            {
                horInput = -1;
            }
        }
        else
        {
            horInput = 0;
        }

        horInput += horAvoidInput;

        velocity = Vector3.Dot(transform.forward, rb.velocity);

        if (velocity < targetSpeed) {
            verInput = 1;
        } else if (velocity > targetSpeed) {
            verInput = -1;
        } else {
            verInput = 0;
        }

        car.SetInput(horInput, verInput);
    }

    private void Sensor() {
        horAvoidInput = 0;

        int numOfRay = Mathf.FloorToInt(sensorAngle / sensorAngleStep);

        for (int i = 0; i < numOfRay; i++) {
            SensorRay(i, rightSensor.position);
            SensorRay(i, midSensor.position);
            SensorRay(-i, midSensor.position);
            SensorRay(-i, leftSensor.position);
        }
    }

    private void SensorRay(int step, Vector3 position)
    {
        Vector3 direction = Quaternion.AngleAxis(step * sensorAngleStep, transform.up) * transform.forward;
        if (Physics.Raycast(position, direction, out RaycastHit hit, sensorDistance, obstacleLayer))
        {
            float A = Mathf.Abs(Vector3.Angle(hit.normal, direction));
            horAvoidInput += -step * (A / baseAngle) * ((sensorDistance - hit.distance) / sensorDistance);
            Debug.DrawLine(position, midSensor.position + direction * sensorDistance, Color.red);
        }
        Debug.DrawLine(position, midSensor.position + direction * sensorDistance, Color.green);
    }

    public void Reset() {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        
        car.CarRigidbody.velocity = Vector3.zero;
        car.CarRigidbody.angularVelocity = Vector3.zero;
    }

}
