using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_BGM;

    void Start()
    {
        int r = 0;
        r = Random.Range(0, m_BGM.Length);

        if(r < m_BGM.Length)
        {
            GameManager.instance.PlayBGM(m_BGM[r]);
        }
        
        if(!GameManager.instance.isOutside())
        {
            this.gameObject.SetActive(false);
        }
    }
}
