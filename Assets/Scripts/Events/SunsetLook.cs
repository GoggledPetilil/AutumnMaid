using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunsetLook : MonoBehaviour
{
    [SerializeField] private GameObject m_SunsetPoint;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            GameManager.instance.SetCamFollower(m_SunsetPoint);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            GameObject playerPoint = GameObject.Find("CamTarget");
            GameManager.instance.SetCamFollower(playerPoint);
        }
    }
}
