using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Cash : MonoBehaviour
{
    [SerializeField] private int amount;
    [SerializeField] private ParticleSystem spray;

    public int Amount => amount;

    private IObjectPool<Cash> objectPool;
    private Renderer renderer;

    public IObjectPool<Cash> ObjectPool { set => objectPool = value; }

    private void Awake() {
        renderer = GetComponent<Renderer>();
    }

    private void OnEnable() {
        renderer.enabled = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            spray.Play();
            renderer.enabled = false;   
            Invoke(nameof(ReturnToPool), 1f);
        }
    }

    private void ReturnToPool() {
        objectPool.Release(this);
        CashSpawner.Ins.updatePosition(transform.position);
    }

}
