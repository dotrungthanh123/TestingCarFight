using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform[] suspensionPoints;
    [SerializeField] private Transform[] driveWheels;
    [SerializeField] private Transform[] frontWheels;

    [Header("Suspension")]
    [SerializeField] private Transform[] wheels;
    [SerializeField] private float restDist;
    [SerializeField] private float travelDist;
    [SerializeField] private float damper;
    [SerializeField] private float power;
    [SerializeField] private float tireRadius;

    [Header("Acceleration")]
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private float breakPower;
    [SerializeField] private float backPower;
    [SerializeField] private float accelPower;
    [SerializeField] private float carTopSpeed;

    [Header("Steering")]
    [SerializeField] private float tireGripFactor;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float rotateSpeed;

    [Header("Input")]
    private float horInput;
    private float verInput;

    [Header("Component")]
    private Rigidbody carRigidbody;
    private Transform carTransform;

    #region Accessor

    public Rigidbody CarRigidbody => carRigidbody;
    public float HorInput => horInput;
    public float VerInput => verInput;
    public float CarTopSpeed => carTopSpeed;
    public float MaxSteeringAngle => maxSteeringAngle;

    #endregion

    #region Unity Functions

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        RotateWheel();
    }

    private void FixedUpdate()
    {
        Suspension();
        Acceleration();
        Steering();
    }

    private void OnDrawGizmos()
    {
        // if (carRigidbody) Visualisation();
    }

    #endregion

    #region Utilities

    public float CurrentSteeringAngle => frontWheels[0].localEulerAngles.y;

    #endregion

    #region Input

    public void SetInput(float horInput, float verInput)
    {
        this.horInput = horInput;
        this.verInput = verInput;
    }

    #endregion

    #region Suspension

    private void Suspension()
    {
        float raycastDistance = restDist + travelDist + tireRadius;

        for (int i = 0; i < suspensionPoints.Length; i++)
        {

            Transform suspensionPoint = suspensionPoints[i];

            Ray ray = new Ray(suspensionPoint.position, -suspensionPoint.up);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {

                Vector3 springDirection = suspensionPoint.up;

                Vector3 tireWorldVelocicty = carRigidbody.GetPointVelocity(suspensionPoint.position);

                float offset = restDist - (hit.distance - tireRadius);

                float springVelocity = Vector3.Dot(springDirection, tireWorldVelocicty);

                float suspensionForce = (offset * power) - (springVelocity * damper);

                carRigidbody.AddForceAtPosition(springDirection * suspensionForce, suspensionPoint.position, ForceMode.Acceleration);

                wheels[i].position =  hit.point + suspensionPoint.up * tireRadius;
            }
            else
            {
                wheels[i].localPosition = (raycastDistance - tireRadius) * -suspensionPoint.up;
            }

        }
    }

    #endregion

    #region Acceleration

    private void Acceleration()
    {

        float raycastDistance = restDist + travelDist + tireRadius;

        foreach (Transform tireTransform in driveWheels)
        {

            Ray ray = new Ray(tireTransform.position, -tireTransform.up);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {

                Vector3 accelerationDirection = tireTransform.forward;

                float carSpeed = Vector3.Dot(carTransform.forward, carRigidbody.velocity);

                if (verInput > 0.0f)
                {
                    if (carSpeed >= 0) {
                        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / CarTopSpeed);

                        float availableTorque = powerCurve.Evaluate(normalizedSpeed) * verInput * accelPower;

                        carRigidbody.AddForceAtPosition(accelerationDirection * availableTorque, tireTransform.position, ForceMode.Acceleration);
                    } else {
                        carRigidbody.AddForceAtPosition(accelerationDirection * breakPower, tireTransform.position, ForceMode.Acceleration);
                    }
                }
                else if (verInput < 0.0f)
                {
                    if (carSpeed > 0)
                    {
                        carRigidbody.AddForceAtPosition(-accelerationDirection * breakPower, tireTransform.position, ForceMode.Acceleration);
                    }
                    else
                    {
                        carRigidbody.AddForceAtPosition(-accelerationDirection * backPower, tireTransform.position, ForceMode.Acceleration);
                    }
                }

            }
        }

    }

    #endregion

    #region Steering

    private void Steering()
    {

        float raycastDistance = restDist + travelDist + tireRadius;

        foreach (Transform suspensionPoint in suspensionPoints)
        {

            Ray ray = new Ray(suspensionPoint.position, -suspensionPoint.up);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {

                Vector3 steeringDir = suspensionPoint.right;

                Vector3 tireWorldVel = carRigidbody.GetPointVelocity(suspensionPoint.position);

                float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);

                float desiredVelChange = -steeringVel * tireGripFactor;

                carRigidbody.AddForceAtPosition(desiredVelChange * steeringDir, suspensionPoint.position, ForceMode.VelocityChange);

            }
        }

    }

    private void RotateWheel()
    {
        // Automatically rotate back to natural position when no inputs given

        foreach (Transform tireTransform in frontWheels)
        {
            float currentSteeringAngle = tireTransform.localEulerAngles.y;

            // rotate left return positive angle instead of negative angle (eg: 359 instead of -1)
            if (currentSteeringAngle > 180) currentSteeringAngle -= 360;

            float newSteeringAngle;

            if (horInput == 0) {
                newSteeringAngle = 0;
            } else {
                newSteeringAngle = currentSteeringAngle + horInput * rotateSpeed * Time.deltaTime;
            }
            tireTransform.localEulerAngles = Mathf.Clamp(newSteeringAngle, -maxSteeringAngle, maxSteeringAngle) * Vector3.up;
        }
    }

    #endregion

    #region Visualisation

    private void Visualisation()
    {
        for (int i = 0; i < suspensionPoints.Length; i++)
        {
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawLine(tireTransforms[i].position, tireVelocity[i] * 10 + tireTransforms[i].position);

            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(tireTransforms[i].position, Vector3.Dot(tireVelocity[i], tireTransforms[i].up) * tireTransforms[i].up + tireTransforms[i].position);

            // Gizmos.color = Color.blue;
            // Gizmos.DrawLine(tireTransforms[i].position, Vector3.Dot(tireVelocity[i], tireTransforms[i].forward) * tireTransforms[i].forward + tireTransforms[i].position);
        }
    }

    #endregion

}