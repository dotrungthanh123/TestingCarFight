using UnityEngine;

public class Car : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform[] frontWheels;
    [SerializeField] private Transform[] rearWheels;
    [SerializeField] private bool frontWheelDrive;

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
    [SerializeField] private AnimationCurve frontWheelGrip;
    [SerializeField] private AnimationCurve rearWheelGrip;
    [SerializeField] private float tireGripFactor;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float rotateSpeed;

    [Header("Input")]
    private float horInput;
    private float verInput;

    [Header("Component")]
    private Rigidbody carRigidbody;
    private Transform carTransform;
    private CarAudio carAudio;

    [Header("Shared")]
    private float raycastDistance;
    private RaycastHit hit;
    private bool started = false;

    #region Accessor

    public Rigidbody CarRigidbody => carRigidbody;
    public float HorInput => horInput;
    public float VerInput => verInput;
    public float CarTopSpeed => carTopSpeed;
    public float MaxSteeringAngle => maxSteeringAngle;
    public float CurrentSteeringAngle => frontWheels[0].localEulerAngles.y;
    public float SpeedRatio => Vector3.Dot(transform.forward, carRigidbody.velocity) / carTopSpeed;

    #endregion

    #region Unity Functions

    private void Awake() {
        carRigidbody = GetComponent<Rigidbody>();
        carTransform = GetComponent<Transform>();
        carAudio = GetComponent<CarAudio>();
    }

    private void Update()
    {
        raycastDistance = tireRadius + restDist + travelDist;
        RotateWheel();
    }

    private void FixedUpdate()
    {
        foreach (Transform wheel in frontWheels) {
            if (WheelRayCast(wheel)) {
                if (frontWheelDrive) Acceleration(wheel);
                ApplyPhysics(wheel);
            }
        }

        foreach (Transform wheel in rearWheels) {
            if (WheelRayCast(wheel)) {
                if (!frontWheelDrive) Acceleration(wheel);
                ApplyPhysics(wheel);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (carRigidbody) Visualisation();
    }

    private void OnCollisionEnter(Collision other) {
        carAudio.Crash();
    }

    #endregion

    #region Utilities

    private bool WheelRayCast(Transform wheel) {
        Ray ray = new Ray(wheel.position, -wheel.up);

        return Physics.Raycast(ray, out hit, raycastDistance);
    }

    private void ApplyPhysics(Transform wheel) {
        Suspension(wheel);
        Steering(wheel);
    }

    #endregion

    #region Input

    public void SetInput(float horInput, float verInput)
    {
        this.horInput = horInput;
        this.verInput = verInput;

        if (verInput > 0 && !started) {
            StartCoroutine(carAudio.StartEngine());
            started = true;
        }
    }

    #endregion

    #region Suspension

    private void Suspension(Transform wheel)
    {
        Vector3 springDirection = wheel.up;

        Vector3 tireWorldVelocicty = carRigidbody.GetPointVelocity(wheel.position);

        float offset = restDist - (hit.distance - tireRadius);

        float springVelocity = Vector3.Dot(springDirection, tireWorldVelocicty);

        float suspensionForce = (offset * power) - (springVelocity * damper);

        carRigidbody.AddForceAtPosition(springDirection * suspensionForce, wheel.position, ForceMode.Acceleration);

        // wheels[i].position =  hit.point + suspensionPoint.up * tireRadius;
        // wheels[i].localPosition = (raycastDistance - tireRadius) * -suspensionPoint.up;
    }

    #endregion

    #region Acceleration

    private void Acceleration(Transform wheel)
    {
        Vector3 accelerationDirection = wheel.forward;

        float carSpeed = Vector3.Dot(carTransform.forward, carRigidbody.velocity);

        if (verInput > 0.0f)
        {
            if (carSpeed >= 0)
            {
                float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / CarTopSpeed);

                float availableTorque = powerCurve.Evaluate(normalizedSpeed) * verInput * accelPower;

                carRigidbody.AddForceAtPosition(accelerationDirection * availableTorque, wheel.position, ForceMode.Acceleration);
            }
            else
            {
                carRigidbody.AddForceAtPosition(accelerationDirection * breakPower, wheel.position, ForceMode.Acceleration);
            }
        }
        else if (verInput < 0.0f)
        {
            if (carSpeed > 0)
            {
                carRigidbody.AddForceAtPosition(-accelerationDirection * breakPower, wheel.position, ForceMode.Acceleration);
            }
            else
            {
                carRigidbody.AddForceAtPosition(-accelerationDirection * backPower, wheel.position, ForceMode.Acceleration);
            }
        }
    }

    #endregion

    #region Steering

    private void Steering(Transform wheel)
    {
        Vector3 steeringDir = wheel.right;

        Vector3 tireWorldVel = carRigidbody.GetPointVelocity(wheel.position);

        float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);

        float desiredVelChange = -steeringVel * tireGripFactor;

        carRigidbody.AddForceAtPosition(desiredVelChange * steeringDir, wheel.position, ForceMode.VelocityChange);
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
        foreach (Transform wheel in frontWheels) {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(wheel.position, Vector3.Dot(wheel.right, carRigidbody.GetPointVelocity(wheel.position)) * wheel.right * 10 + wheel.position);
        }

        foreach (Transform wheel in rearWheels) {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(wheel.position, Vector3.Dot(wheel.right, carRigidbody.GetPointVelocity(wheel.position)) * wheel.right * 10 + wheel.position);
        }
    }

    #endregion

    #region Friction
        
    private void Friction() {
        
    }

    #endregion

}