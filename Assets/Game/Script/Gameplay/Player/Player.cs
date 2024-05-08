using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    private Car car;
    private float horInput, verInput;
    private PlayerInfo playerInfo;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake() {
        car = GetComponent<Car>();
        playerInfo = GetComponent<PlayerInfo>();

        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }
    private void Update() {
        GetPlayerInput();
        car.SetInput(horInput, verInput);
    }
    
    private void GetPlayerInput() {
        horInput = Input.GetAxis("Horizontal");
        verInput = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Cash") {
            Cash cash = other.GetComponent<Cash>();
            playerInfo.AddCash(cash.Amount);
        }
    }

    public void Reset() {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        
        car.CarRigidbody.velocity = Vector3.zero;
        car.CarRigidbody.angularVelocity = Vector3.zero;
    }

}
