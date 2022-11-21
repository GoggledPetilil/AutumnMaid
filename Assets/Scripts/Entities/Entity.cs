using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    
    [SerializeField] protected Transform m_srHolder;
    [SerializeField] protected bool m_Flipped;
    private bool m_Flipping;    // Character is still in the process of flipping.
    [SerializeField] private AudioSource m_Aud;
    [SerializeField] private AudioClip m_SpinSFX;

    public bool isFacingRight()
    {
        return !m_Flipped;
    }

    public void SqueezeSprite(float squeezeX, float squeezeY, float sec)
    {
        StopCoroutine(Squeeze(squeezeX, squeezeY, sec));
        StartCoroutine(Squeeze(squeezeX, squeezeY, sec));
    }

    public void FlipSprite(bool state)
    {
        if(m_Flipped == state) return;
        
        float startY = 180.0f;
        float endY = 0.0f;
        float dur = 0.2f;
        if(state == true)
        {
            startY = 0.0f;
            endY = 180.0f;
        }

        m_Flipped = state;

        StopCoroutine(FlipCharacter(startY, endY, dur));
        StartCoroutine(FlipCharacter(startY, endY, dur));
    }

    public void SpinSprite()
    {
        float startY = 360.0f * 1.5f;
        float endY = 0.0f;
        float dur = 0.45f;
        if(m_Flipped == true)
        {
            startY = 0.0f;
            endY = 180.0f * 3f;
        }

        m_Aud.clip = m_SpinSFX;
        if(m_Flipping == false) m_Aud.Play();

        StopCoroutine(FlipCharacter(startY, endY, dur));
        StartCoroutine(FlipCharacter(startY, endY, dur));
    }

    private IEnumerator FlipCharacter(float startRot, float endRot, float dur)
    {
        float startY = startRot;
        float endY = endRot;
        float flipDur = dur;
        float t = 0.0f;
        m_Flipping = true;

        while(t < 1f)
        {
            t += Time.deltaTime / flipDur;
            float rotY = Mathf.Lerp(startY, endY, t);
            m_srHolder.transform.rotation = Quaternion.Euler(m_srHolder.transform.rotation.x, rotY, m_srHolder.transform.rotation.z);
            yield return null;
        }

        yield return null;
        m_Flipping = false;
    }

    private IEnumerator Squeeze(float squeezeX, float squeezeY, float sec)
    {
        Vector3 originSize = Vector3.one;
        Vector3 newSize = new Vector3(squeezeX, squeezeY, originSize.z);
        float t = 0f;
        
        Animator anim = GetComponent<Animator>();

        anim.enabled = false;
        while (t <= 1f)
        {
            t += Time.deltaTime / sec;
            m_srHolder.gameObject.transform.localScale = Vector3.Lerp(originSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / sec;
            m_srHolder.gameObject.transform.localScale = Vector3.Lerp(newSize, originSize, t);
            yield return null;
        }
        anim.enabled = true;
    }
}
