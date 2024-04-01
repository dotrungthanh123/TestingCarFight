using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{

    [Header("Wheel")]
    public Transform[] driveWheels;
    public Transform[] frontWheels;
    public Vector3[] contactPoints;
    public Transform[] wheelsTransform;

    [Header("Debug")]
    public Vector3[] tireVelocity;
    public float steeringVel;

    [Header("Suspension")]
    public Transform[] suspensionPoints;
    public float suspensionRestDistance;
    public float suspensionTravelDistance;
    public float springDamper;
    public float springStrength;
    public float tireRadius;

    [Header("Acceleration")]
    public AnimationCurve powerCurve;
    public float breakPower;
    public float backPower;
    public float accelPower;
    public float carTopSpeed;

    [Header("Steering")]
    public float tireGripFactor;
    public float maxSteeringAngle;
    public float rotateSpeed;

    [Header("Input")]
    private float horInput;
    private float verInput;

    [Header("Component")]
    private Rigidbody carRigidbody;
    private Transform carTransform;

    #region Unity Functions

    private void Start()
    {
        tireVelocity = new Vector3[4];

        carRigidbody = GetComponent<Rigidbody>();
        carTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        GetPlayerInput();
        // RotateWheel();
    }

    private void FixedUpdate()
    {
        Suspension();
        Acceleration();
        // Steering();
    }

    private void OnDrawGizmos()
    {
        if (carRigidbody)
        {
            Visualisation();
        }
    }

    #endregion

    #region Utilities

    #endregion

    #region Player Input

    private void GetPlayerInput()
    {
        horInput = Input.GetAxisRaw("Horizontal");
        verInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) Debug.Break();
    }

    #endregion

    #region Suspension

    private void Suspension()
    {

        float raycastDistance = suspensionRestDistance + suspensionTravelDistance + tireRadius;

        for (int i = 0; i < suspensionPoints.Length; i++)
        {
            Transform suspensionPoint = suspensionPoints[i];

            Ray ray = new Ray(suspensionPoint.position, -suspensionPoint.up);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {
                contactPoints[i] = hit.point;
                
                Vector3 springDirection = suspensionPoint.up;

                Vector3 tireWorldVelocicty = carRigidbody.GetPointVelocity(suspensionPoint.position);

                float offset = suspensionRestDistance - (hit.distance - tireRadius);

                float springVelocity = Vector3.Dot(springDirection, tireWorldVelocicty);

                float dampForce = (offset * springStrength) - (springVelocity * springDamper);

                carRigidbody.AddForceAtPosition(springDirection * dampForce, contactPoints[i]);

                Debug.DrawLine(contactPoints[i], contactPoints[i] + suspensionPoint.up * dampForce);

                wheelsTransform[i].position = contactPoints[i] + suspensionPoint.up * tireRadius;
            }
            else
            {
                wheelsTransform[i].position = suspensionPoint.position - suspensionPoint.up * (raycastDistance - tireRadius);
            }
        }
    }

    #endregion

    #region Acceleration

    // car's center of gravity shifts towards the rear as a result of acceleration, causing the front suspension to compress and the rear suspension to extend, resulting in the front end lifting up.

    // front-wheel drive cars cause less lifting compared to rear-wheel drive cars. lower center of gravity and stiffer suspension experience less lifting

    private void Acceleration()
    {

        float raycastDistance = suspensionRestDistance + suspensionTravelDistance + tireRadius;

        foreach (Transform tireTransform in driveWheels)
        {

            Ray ray = new Ray(tireTransform.position, -tireTransform.up);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {

                Vector3 accelerationDirection = tireTransform.forward;

                float carSpeed = Vector3.Dot(carTransform.forward, carRigidbody.velocity);

                if (verInput > 0.0f)
                {

                    float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

                    float availableTorque = powerCurve.Evaluate(normalizedSpeed) * verInput * accelPower;

                    carRigidbody.AddForceAtPosition(accelerationDirection * availableTorque, tireTransform.position, ForceMode.Acceleration);

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

        float raycastDistance = suspensionRestDistance + suspensionTravelDistance + tireRadius;

        foreach (Transform tireTransform in suspensionPoints)
        {

            Ray ray = new Ray(tireTransform.position, -tireTransform.up);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {

                Vector3 steeringDir = tireTransform.right;

                Vector3 tireWorldVel = carRigidbody.GetPointVelocity(tireTransform.position);

                steeringVel = Vector3.Dot(steeringDir, tireWorldVel);

                float desiredVelChange = -steeringVel * tireGripFactor;

                float desiredAccel = desiredVelChange / Time.fixedDeltaTime;

                carRigidbody.AddForceAtPosition(steeringDir * desiredAccel, tireTransform.position);

            }
        }

        Debug.DrawLine(carTransform.position, carRigidbody.velocity + carTransform.position, Color.blue);

    }

    private void RotateWheel()
    {
        // Automatically rotate back to natural position when no inputs given

        foreach (Transform tireTransform in frontWheels)
        {
            float currentSteeringAngle = tireTransform.localEulerAngles.y;

            // rotate left return positive angle instead of negative angle (eg: 359 instead of -1)
            if (currentSteeringAngle > 180) currentSteeringAngle -= 360;

            float rotateAngle = horInput != 0 ? (horInput * rotateSpeed * Time.deltaTime) : 0 ;
            float newSteeringAngle = rotateAngle + currentSteeringAngle;

            tireTransform.localEulerAngles = Mathf.Clamp(newSteeringAngle, -maxSteeringAngle, maxSteeringAngle) * Vector3.up;
        }
    }

    #endregion

    #region Visualisation

    private void Visualisation()
    {
        for (int i = 0; i < suspensionPoints.Length; i++)
        {
            tireVelocity[i] = carRigidbody.GetPointVelocity(suspensionPoints[i].position);

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