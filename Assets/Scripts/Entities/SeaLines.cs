using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaLines : MonoBehaviour
{
    [SerializeField] private Transform m_FollowObject;

    void Update()
    {
        transform.position = m_FollowObject.position;
    }
}
