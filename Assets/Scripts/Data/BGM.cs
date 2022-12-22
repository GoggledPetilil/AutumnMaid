using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_BGM;
    [SerializeField] private bool m_isOutside;
    private float m_MusicTime = 64;     // Time between each song.
    private float m_NextSong;           // Time until next song.

    void Start()
    {
        int r = 0;
        if(m_isOutside)
        {
            r = Random.Range(0, m_BGM.Length+1);
        }
        else
        {
            r = Random.Range(0, m_BGM.Length);
        }
        if(r < m_BGM.Length)
        {
            GameManager.instance.PlayBGM(m_BGM[r]);
        }
        
        if(!m_isOutside)
        {
            this.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if(Time.time > m_NextSong && m_isOutside)
        {
            int r = Random.Range(0, m_BGM.Length);
            GameManager.instance.PlayBGM(m_BGM[r]);
            m_NextSong = Time.time + m_BGM[r].length + m_MusicTime;
        }
    }
}
