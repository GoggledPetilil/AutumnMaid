using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanTrigger : MonoBehaviour
{
    public float m_Distance;
    [SerializeField] private AudioSource m_Audio;
    private float m_OceanPoint;
    private GameObject m_Player;

    void Awake()
    {
        m_Audio.volume = 0.0f;
        m_OceanPoint = this.transform.position.y;
    }
    
    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(m_OceanPoint < m_Player.transform.position.y)
        {
            m_Audio.volume = 1.0f - Mathf.Clamp(Mathf.Abs(m_OceanPoint - m_Player.transform.position.y) / m_Distance, 0.0f, 1.0f);
        }
        else
        {
            m_Audio.volume = 1.0f;
        }
    }
}
