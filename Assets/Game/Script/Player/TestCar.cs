using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCar : MonoBehaviour
{

    public Rigidbody rigidbody;

    private float vertical, horizontal;

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        vertical = Input.GetAxis("Vertical");
        Physics.Raycast(transform.position + transform.up * 100, transform.up, out RaycastHit hit, 1);
    }

    private void FixedUpdate() {
        float forwardSpeed = Vector3.Dot(transform.forward, rigidbody.velocity);


        if (forwardSpeed != 0) {
            rigidbody.AddForce(-forwardSpeed * transform.forward, ForceMode.VelocityChange);
        }
    }
}
