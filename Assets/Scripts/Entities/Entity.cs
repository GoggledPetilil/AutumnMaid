using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    
    [SerializeField] protected Transform m_srHolder;
    protected bool m_Flipped;

    public void FlipSprite(bool state)
    {
        if(m_Flipped == state) return;
        
        float startY = 180.0f;
        float endY = 0.0f;
        if(state == true)
        {
            startY = 0.0f;
            endY = 180.0f;
        }

        m_Flipped = state;
        //if(m_CanFlip == false) return;

        StopCoroutine(FlipCharacter(startY, endY));
        StartCoroutine(FlipCharacter(startY, endY));
    }

    private IEnumerator FlipCharacter(float startRot, float endRot)
    {
        float startY = startRot;
        float endY = endRot;
        float flipDur = 0.2f;
        float t = 0.0f;

        while(t < 1f)
        {
            t += Time.deltaTime / flipDur;
            float rotY = Mathf.Lerp(startY, endY, t);
            m_srHolder.transform.rotation = Quaternion.Euler(m_srHolder.transform.rotation.x, rotY, m_srHolder.transform.rotation.z);
            yield return null;
        }

        yield return null;
    }

    public IEnumerator Squeeze(float squeezeX, float squeezeY, float sec)
    {
        Vector3 originSize = Vector3.one;
        Vector3 newSize = new Vector3(squeezeX, squeezeY, originSize.z);
        float t = 0f;
        
        Animator anim = GetComponent<Animator>();
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        anim.enabled = false;
        while (t <= 1f)
        {
            t += Time.deltaTime / sec;
            sr.gameObject.transform.localScale = Vector3.Lerp(originSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / sec;
            sr.gameObject.transform.localScale = Vector3.Lerp(newSize, originSize, t);
            yield return null;
        }
        anim.enabled = true;
    }
}
