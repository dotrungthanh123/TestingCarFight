using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [SerializeField] private AudioSource runSound;
    [SerializeField] private float runMaxVolume;
    [SerializeField] private float runMaxPitch;
    [SerializeField] private AudioSource reverseSound;
    [SerializeField] private float reverseMaxVolume;
    [SerializeField] private float reverseMaxPitch;
    [SerializeField] private AudioSource idleSound;
    [SerializeField] private float idleMaxVolume;
    [SerializeField] private AudioSource startSound;
    [SerializeField] private float startVolume;
    [SerializeField] private AudioSource crashSound;
    [SerializeField] private float crashVolume;

    private Car car;
    private float speedRatio;
    private bool isEngineRunning = false;

    private void Start() {
        car = GetComponent<Car>();
        idleSound.volume = 0;
        runSound.volume = 0;
        startSound.volume = startVolume;
        crashSound.volume = crashVolume;
        runSound.Play();
        idleSound.Play();
    }    

    void Update()
    {
        float speedSign = 0;
        if (car)
        {
            speedSign = Mathf.Sign(car.SpeedRatio);
            speedRatio = Mathf.Abs(car.SpeedRatio);
        }
        if (isEngineRunning)
        {
            idleSound.volume = Mathf.Lerp(0.1f, idleMaxVolume, speedRatio);
            if (speedSign > 0)
            {
                reverseSound.volume = 0;
                runSound.volume = Mathf.Lerp(0.3f, runMaxVolume, speedRatio);
                runSound.pitch = Mathf.Lerp(runSound.pitch, Mathf.Lerp(0.3f, runMaxPitch, speedRatio), Time.deltaTime);
            }
            else
            {
                runSound.volume = 0;
                reverseSound.volume = Mathf.Lerp(0f, reverseMaxVolume, speedRatio);
                reverseSound.pitch = Mathf.Lerp(reverseSound.pitch, Mathf.Lerp(0.2f, reverseMaxPitch, speedRatio), Time.deltaTime);
            }
        }
        else {
            idleSound.volume = 0;
            runSound.volume = 0;
        }
    }

    public IEnumerator StartEngine()
    {
        startSound.Play();
        yield return new WaitForSeconds(0.6f);
        isEngineRunning = true;
        yield return new WaitForSeconds(0.4f);
    }

    public void Crash() {
        crashSound.Play();        
    }

}
