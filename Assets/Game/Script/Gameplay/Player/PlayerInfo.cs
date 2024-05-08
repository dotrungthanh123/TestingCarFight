using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI cashText;

    private int cash;

    public int Cash => cash;

    private void Awake() {
        cash = PlayerPrefs.GetInt("Cash", 0);
        cashText.text = cash.ToString();
    }

    public int AddCash(int amount) {
        cash += amount;
        cashText.text = cash.ToString();
        return cash;
    }

    public int ReduceCash(int amount) {
        cash -= amount;
        cashText.text = cash.ToString();
        return cash;
    }

    private void OnApplicationQuit() {
        PlayerPrefs.SetInt("Cash", cash);
    }
}