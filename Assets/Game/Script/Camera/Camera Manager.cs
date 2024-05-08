using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private Animator dollyAnimator;

    public void OnPlay() {
        dollyAnimator.SetTrigger("Intro");
    }
}
