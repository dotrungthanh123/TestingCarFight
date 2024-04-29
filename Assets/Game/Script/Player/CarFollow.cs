using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFollow : MonoBehaviour
{
    [SerializeField] private Car target;
    [Tooltip("Proportion of the car max speed")]
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
    private float horAvoidInput;

    private Car car;
    private Rigidbody rb;
    private float horInput, verInput;
    private Vector3 targetLocalPosition;
    private float currentSteeringAngle;
    private float steerDir;
    private float targetRotationAngle;
    private float currentCarSteeringAngle;

    private void Start() {
        car = GetComponent<Car>();   
        rb = GetComponent<Rigidbody>();
    }

    public void SetTarget(Car car) {
        target = car;
    }

    private void Update() {
        float squaredTargetSpeed = (targetSpeed * car.carTopSpeed) * (targetSpeed * car.carTopSpeed);

        // Transform target world position to bot local position
        targetLocalPosition = transform.InverseTransformPoint(
            target.transform.position.x,
            transform.position.y,
            target.transform.position.z);

        // The car move forward on z axis and steer on x axis
        // Use the x value on the magnitude gives the direction to turn left or right
        // Equation does not specifically calculating how much to steer but more on steering left or right

        currentCarSteeringAngle = car.CurrentSteeringAngle;

        currentSteeringAngle = car.CurrentSteeringAngle > car.maxSteeringAngle + 1 ? 
                                                            car.CurrentSteeringAngle - 360 : car.CurrentSteeringAngle;

        steerDir = targetLocalPosition.x / targetLocalPosition.magnitude;
        targetRotationAngle = car.maxSteeringAngle * steerDir;
        
        if (targetRotationAngle > currentSteeringAngle)
        {
            horInput = 1;
        } else if (targetRotationAngle < currentSteeringAngle)
        {
            horInput = -1;
        } else {
            horInput = 0;
        }

        Sensor();

        horInput += Mathf.Clamp(horAvoidScaler * horAvoidInput, -3, 3);

        float squaredVelocity = rb.velocity.x * rb.velocity.x + rb.velocity.y * rb.velocity.y;

        if (squaredVelocity < squaredTargetSpeed) {
            verInput = 1;
        } else if (squaredVelocity > squaredTargetSpeed) {
            verInput = -1;
        } else {
            verInput = 0;
        }

        car.SetInput(horInput, verInput);
    }

    private void Sensor() {
        horAvoidInput = 0;
        // Angle use to calculate how much to increase horAvoidInput
        float baseAngle = 180;

        int numOfRay = Mathf.FloorToInt(sensorAngle / sensorAngleStep);

        for (int i = 0; i < numOfRay; i++) {
            Vector3 direction;
            RaycastHit hit;
            // origion - direction - hit - distance - layer

            // left
            direction = Quaternion.AngleAxis(-i * sensorAngleStep, transform.up) * transform.forward;
            if (Physics.Raycast(leftSensor.position, direction, out hit, sensorDistance, obstacleLayer)) {
                // Angle A = angle between normal and direction
                // |A| proportional to angle required to rotate
                
                Debug.DrawLine(leftSensor.position, leftSensor.position + direction * sensorDistance, Color.red);

                float A = Mathf.Abs(Vector3.Angle(hit.normal, direction));
                horAvoidInput += A / baseAngle;
            } else {
                Debug.DrawLine(leftSensor.position, leftSensor.position + direction * sensorDistance);
            }

            // mid left
            if (Physics.Raycast(midSensor.position, direction, out hit, sensorDistance, obstacleLayer)) {
                // Angle A = angle between normal and direction
                // |A| proportional to angle required to rotate
                
                Debug.DrawLine(midSensor.position, midSensor.position + direction * sensorDistance, Color.red);

                float A = Mathf.Abs(Vector3.Angle(hit.normal, direction));
                horAvoidInput += A / baseAngle;
            } else {
                Debug.DrawLine(midSensor.position, midSensor.position + direction * sensorDistance);
            }

            // right
            direction = Quaternion.AngleAxis(i * sensorAngleStep, transform.up) * transform.forward;
            if (Physics.Raycast(rightSensor.position, direction, out hit, sensorDistance, obstacleLayer)) {
                // Angle A = angle between normal and direction
                // |A| proportional to angle required to rotate

                Debug.DrawLine(rightSensor.position, rightSensor.position + direction * sensorDistance, Color.red);

                float A = Mathf.Abs(Vector3.Angle(hit.normal, direction));
                horAvoidInput -= A / baseAngle;
            } else {
                Debug.DrawLine(rightSensor.position, rightSensor.position + direction * sensorDistance);
            }

            // mid right
            if (Physics.Raycast(midSensor.position, direction, out hit, sensorDistance, obstacleLayer)) {
                // Angle A = angle between normal and direction
                // |A| proportional to angle required to rotate

                Debug.DrawLine(midSensor.position, midSensor.position + direction * sensorDistance, Color.red);

                float A = Mathf.Abs(Vector3.Angle(hit.normal, direction));
                horAvoidInput -= A / baseAngle;
            } else {
                Debug.DrawLine(midSensor.position, midSensor.position + direction * sensorDistance);
            }
        }
    }
}
