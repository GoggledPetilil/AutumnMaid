using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    [SerializeField] private GameObject m_AfterImagePrefab;
    private Queue<GameObject> m_AvailableObjects = new Queue<GameObject>();

    public static AfterImagePool instance { get; private set; }

    void Awake()
    {
        instance = this;
        GrowPool();
    }

    private void GrowPool()
    {
        for(int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(m_AfterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        m_AvailableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if(m_AvailableObjects.Count == 0)
        {
            GrowPool();
        }
        
        var instance = m_AvailableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
