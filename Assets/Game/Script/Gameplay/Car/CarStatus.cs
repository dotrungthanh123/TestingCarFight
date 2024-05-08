using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarStatus : MonoBehaviour
{

    [SerializeField] private Slider hpSlider;
    [SerializeField] private float sliderSpeed;
    [SerializeField] private LayerMask obstacleLayer;

    private float hp;
    private float maxHp;

    private void Start() {
        maxHp = 100;
    }

    private void Update() {
        hpSlider.value = Mathf.Lerp(hpSlider.value, hp / maxHp, Time.deltaTime * sliderSpeed);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == 3) {
            hp -= 5;
        } else if (other.gameObject.tag == "Bot") {
            hp -= 15; 
        }
        
        if (hp <= 0) {
            BotManager.Ins.OnEnd();
            GameManager.Ins.OnEnd();
        }

    }
    
    public void OnPlay() {
        hp = 300;
    }
}
