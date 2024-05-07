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
    [SerializeField] private float idleMaxPitch;
    [SerializeField] private float LimiterSound = 1f;
    [SerializeField] private float LimiterFrequency = 3f;
    [SerializeField] private float LimiterEngage = 0.8f;
    [SerializeField] private bool isEngineRunning = false;

    public AudioSource startingSound;

    private float revLimiter;
    private float speedRatio;

    private Car car;

    private void Start() {
        car = GetComponent<Car>();
    }    

    void Update()
    {
        float speedSign = 0;
        if (car)
        {
            speedSign = Mathf.Sign(car.SpeedRatio);
            speedRatio = Mathf.Abs(car.SpeedRatio);
        }
        if (speedRatio > LimiterEngage)
        {
            revLimiter = (Mathf.Sin(Time.time * LimiterFrequency) + 1f) * LimiterSound * (speedRatio - LimiterEngage);
        }
        if (isEngineRunning)
        {
            idleSound.volume = Mathf.Lerp(0.1f, idleMaxVolume, speedRatio);
            if (speedSign > 0)
            {
                reverseSound.volume = 0;
                runSound.volume = Mathf.Lerp(0.3f, runMaxVolume, speedRatio);
                runSound.pitch = Mathf.Lerp(runSound.pitch, Mathf.Lerp(0.3f, runMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
            }
            else
            {
                runSound.volume = 0;
                reverseSound.volume = Mathf.Lerp(0f, reverseMaxVolume, speedRatio);
                reverseSound.pitch = Mathf.Lerp(reverseSound.pitch, Mathf.Lerp(0.2f, reverseMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
            }
        }
        else {
            idleSound.volume = 0;
            runSound.volume = 0;
        }
    }
    public IEnumerator StartEngine()
    {
        startingSound.Play();
        car.IsEngineRunning = 1;
        yield return new WaitForSeconds(0.6f);
        isEngineRunning = true;
        yield return new WaitForSeconds(0.4f);
        car.IsEngineRunning = 2;
    }


}
