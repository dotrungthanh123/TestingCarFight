using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarList", menuName = "CarList", order = 0)]
public class CarList : ScriptableObject {

    public List<CarInfo> cars;

    public CarInfo GetCar(int index) => cars[index];
    public int Count => cars.Count;

}