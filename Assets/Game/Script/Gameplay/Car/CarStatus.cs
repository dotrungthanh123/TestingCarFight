using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarStatus : MonoBehaviour
{

    [SerializeField] private Slider hpSlider;
    [SerializeField] private float sliderSpeed;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float maxHp;

    private float hp;

    private void Update() {
        hpSlider.value = Mathf.Lerp(hpSlider.value, hp / maxHp, Time.deltaTime * sliderSpeed);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == 3) {
            hp -= 10;
        } else if (other.gameObject.tag == "Bot") {
            hp -= 20; 
        }
        
        if (hp <= 0) {
            BotManager.Ins.OnEnd();
            GameManager.Ins.OnEnd();
            UIManager.Ins.OnEnd();
        }

    }
    
    public void OnPlay() {
        hp = maxHp;
    }
}
