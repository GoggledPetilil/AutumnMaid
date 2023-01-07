using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : Entity
{
    [SerializeField] private Animator m_Anim;
    [SerializeField] private SpriteRenderer m_sr;
    [SerializeField] private SpriteRenderer m_shadow;
    [SerializeField] private bool isRunning;

    void Awake()
    {
        int r = Random.Range(0,8);
        if(r != 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player") && !isRunning)
        {
            isRunning = true;
            StartCoroutine(RunAway());
        }
    }

    IEnumerator RunAway()
    {
        Vector2 startPos = this.transform.position;
        float distance = 4.0f;
        Vector2 endPos = new Vector2(startPos.x + distance, startPos.y);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player.transform.position.x > this.transform.position.x)
        {
            endPos = new Vector2(startPos.x - distance, startPos.y);
            FlipSprite(true);
        }

        Color startColor = new Color(m_sr.color.r, m_sr.color.g, m_sr.color.b, 1.0f);
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.0f);
        Color shadowStart = new Color(m_shadow.color.r, m_shadow.color.g, m_shadow.color.b, m_shadow.color.a);
        Color shadowEnd = new Color(shadowStart.r, shadowStart.g, shadowStart.b, 0.0f);

        float t = 0.0f;
        float walkDur = Vector2.Distance(startPos, endPos) / 3.2f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / walkDur;

            transform.position = Vector2.Lerp(startPos, endPos, t);
            m_sr.color = Color.Lerp(startColor, endColor, t);
            m_shadow.color = Color.Lerp(shadowStart, shadowEnd, t);

            m_Anim.SetBool("isWalking", true);
            yield return null;
        }

        this.gameObject.SetActive(false);
    }
}
