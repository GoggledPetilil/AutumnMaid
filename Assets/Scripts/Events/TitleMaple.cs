using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMaple : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_sr;
    [SerializeField] private AudioSource m_aud;
    [SerializeField] private AudioClip m_growSfx;
    [SerializeField] private AudioClip m_shrinkSfx;

    void OnMouseDown()
    {
        bool state = !GameManager.instance.m_BigMode;
        Vector2 size = Vector2.one;
        
        if(state == true)
        {
            size = new Vector2(2.0f, size.y);
            m_aud.clip = m_growSfx;
        }
        else 
        {
            m_aud.clip = m_shrinkSfx;
        }
        
        m_sr.transform.localScale = size;
        GameManager.instance.m_BigMode = state;
        m_aud.Play();
    }
}
