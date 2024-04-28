using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    private Car car;
    private float horInput, verInput;

    private void Start() {
        car = GetComponent<Car>();
    }

    private void Update() {
        GetPlayerInput();
        car.SetInput(horInput, verInput);
    }
    
    private void GetPlayerInput() {
        horInput = Input.GetAxis("Horizontal");
        verInput = Input.GetAxis("Vertical");
    }

}
