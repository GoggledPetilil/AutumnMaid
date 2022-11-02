using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    void Start()
    {
        Camera.main.gameObject.TryGetComponent<CinemachineBrain>(out var brain);
        if(brain == null)
        {
            brain = Camera.main.gameObject.AddComponent<CinemachineBrain>();
        }
        CinemachineVirtualCamera vcam = gameObject.GetComponentInChildren<CinemachineVirtualCamera>();

        Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().m_TargetPoint.transform;
        
        vcam.m_Follow = player;
    }
}
