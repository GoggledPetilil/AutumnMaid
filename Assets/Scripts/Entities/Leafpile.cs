using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leafpile : Destructable
{
    private Animator m_Anim;
    private AudioSource m_Aud;
    [SerializeField] private ParticleSystem m_TouchParticles;
    [SerializeField] private ParticleSystem m_DeathParticles;
    [SerializeField] private GameObject m_SpriteHolder;
    [SerializeField] private GameObject m_SpriteShadow;
    [SerializeField] private AudioClip m_ShakeSFX;
    [SerializeField] private AudioClip m_DestroySFX;
    private bool m_IsActive;

    void Awake()
    {
        m_Anim = GetComponent<Animator>();
        m_Aud = GetComponent<AudioSource>();
    }

    void Start()
    {
        m_Aud.clip = m_ShakeSFX;
        m_IsActive = true;
    }

    public override void GetHit()
    {
        if(!m_IsActive) return;
        
        m_SpriteHolder.SetActive(false);
        m_SpriteShadow.SetActive(false);
        
        m_Aud.clip = m_DestroySFX;
        m_Aud.pitch = Random.Range(0.8f, 1.0f);
        m_Aud.Play();

        m_DeathParticles.Play();

        m_IsActive = false;
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player") && m_IsActive)
        {
            m_TouchParticles.Play();

            m_Aud.pitch = Random.Range(0.8f, 1.0f);
            m_Aud.Play();
            
            m_Anim.SetTrigger("Shake");
        }
    }
}
