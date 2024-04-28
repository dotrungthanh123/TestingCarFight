using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarInfo", menuName = "CarInfo", order = 0)]
public class CarInfo : ScriptableObject {
    public string name;
    public int cost;
    public GameObject model;
}