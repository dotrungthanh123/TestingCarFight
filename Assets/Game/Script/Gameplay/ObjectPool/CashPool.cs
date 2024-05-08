using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CashPool : MonoBehaviour
{

    [SerializeField] private Cash[] cashPrefabs;

    private ObjectPool<Cash> objectPool;

    private void Awake()
    {
        objectPool = new ObjectPool<Cash>(Create,
            OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject);
    }

    private Cash Create()
    {
        Cash cash = Instantiate(cashPrefabs[Random.Range(0, cashPrefabs.Length)]);
        cash.ObjectPool = objectPool;
        return cash;
    }

    // invoked when returning an item to the object pool
    private void OnReleaseToPool(Cash pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }

    // invoked when retrieving the next item from the object pool
    private void OnGetFromPool(Cash pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }

    // invoked when we exceed the maximum number of pooled items (i.e. destroy the pooled object)
    private void OnDestroyPooledObject(Cash pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }

    public Cash Get() {
        return objectPool.Get();
    }
}
