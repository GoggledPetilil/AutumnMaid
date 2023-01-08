using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchEvent : MonoBehaviour
{
    [SerializeField] private bool m_FaceRight;
    [SerializeField] private bool m_FaceLeft;
    [SerializeField] private Transform m_SitPosition;
    [SerializeField] private Transform m_StandPosition;
    private bool m_IsSitting;
    private bool m_InAnimation;

    [Header("Special Components")]
    [SerializeField] private Quest m_QuestData;
    [SerializeField] private Item m_TeaItem;
    public bool m_IsSpecial;    // On this beach, you drink.
    public int m_Minutes;       // How many minutes you gotta wait.
    private float m_WaitTimer;

    void Update()
    {
        if(!m_IsSpecial) return;

        if(m_IsSitting && GameManager.instance.m_FlagHaveTea && !GameManager.instance.m_FlagDrankTea)
        {
            if(Time.time > m_WaitTimer)
            {
                Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                player.m_ani.SetBool("isDrinking", false);

                GameManager.instance.m_FlagDrankTea = true;
                GameManager.instance.IncreaseHappiness();
                GameManager.instance.CompleteQuest(m_QuestData);
                GameManager.instance.RemoveItem(m_TeaItem);
            }
        }
    }

    public void SitEvent()
    {
        if(m_InAnimation || GameManager.instance.m_IsDelivering) return;

        SetWaitTimer();     // Wait timer is set when the player gets off,
                            // To prevent the quest from being completed while getting off.

        if(!m_IsSitting)
        {
            StartCoroutine(SitDown());
        }
        else 
        {
            StartCoroutine(GetOff());
        }
    }

    IEnumerator SitDown()
    {
        m_InAnimation = true;

        // Stop Player Movement
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.StopMovement(true, false);
        player.DisableColliders(true);

        yield return new WaitForSeconds(0.1f);

        // Move Player towards door
        float t = 0.0f;
        Vector2 startPos = player.transform.position;
        Vector2 endPos = m_SitPosition.position;
        float movDur = Vector2.Distance(startPos, endPos) / 5.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / movDur;
            player.transform.position = Vector2.Lerp(startPos, endPos, t);
            player.m_ani.SetBool("isMoving", true);
            yield return null;
        }
        player.m_ani.SetBool("isMoving", false);
        player.m_ani.SetBool("isSitting", true);
        if(m_IsSpecial && GameManager.instance.m_FlagHaveTea && !GameManager.instance.m_FlagDrankTea)
        {
            player.m_ani.SetBool("isDrinking", true);
        }

        if(m_FaceRight && !m_FaceLeft)
        {
            player.FlipSprite(false);
        }
        else if(m_FaceLeft && !m_FaceRight)
        {
            player.FlipSprite(true);
        }
        else if(m_FaceLeft && m_FaceRight)
        {
            int r = Random.Range(0,2);
            if(r == 0)
            {
                player.FlipSprite(false);
            }
            else 
            {
                player.FlipSprite(true);
            }
        }

        player.m_InteractEvent = GetComponentInChildren<Interactable>();
        m_IsSitting = true;
        m_InAnimation = false;
    }

    IEnumerator GetOff()
    {
        m_InAnimation = true;
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        yield return new WaitForSeconds(0.1f);

        // Move Player towards door
        float t = 0.0f;
        Vector2 startPos = m_SitPosition.position;
        Vector2 endPos = m_StandPosition.position;
        float movDur = 0.2f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / movDur;
            player.transform.position = Vector2.Lerp(startPos, endPos, t);
            if(t > 0.5f)
            {
                player.m_ani.SetBool("isSitting", false);
                player.m_ani.SetBool("isDrinking", false);
                player.m_ani.SetBool("isMoving", true);
            }
            yield return null;
        }
        player.m_ani.SetBool("isMoving", false);

        // Resume movement
        player.StopMovement(false);
        player.DisableColliders(false);

        m_IsSitting = false;
        m_InAnimation = false;
    }

    void SetWaitTimer()
    {
        m_WaitTimer = Time.time + (m_Minutes * 60);
    }
}
