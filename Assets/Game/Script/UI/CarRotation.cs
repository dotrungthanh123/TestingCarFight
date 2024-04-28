using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRotation : MonoBehaviour
{

    [SerializeField] private float rotateSpeed;

    private void Update() {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

}
