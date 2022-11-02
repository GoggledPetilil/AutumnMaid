using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private Animator m_Anim;
    private AudioSource m_Aud;
    private ParticleSystem m_PS;
    [SerializeField] private AudioClip m_ShakeSFX;

    void Awake()
    {
        m_Anim = GetComponent<Animator>();
        m_Aud = GetComponent<AudioSource>();
        m_PS = GetComponentInChildren<ParticleSystem>();
    }

    void Start()
    {
        m_Aud.clip = m_ShakeSFX;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            m_PS.Play();

            m_Aud.pitch = Random.Range(0.8f, 1.0f);
            m_Aud.Play();
            
            m_Anim.SetTrigger("Shake");
        }
    }
}
