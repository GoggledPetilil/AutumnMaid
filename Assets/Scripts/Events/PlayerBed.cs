using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBed : MonoBehaviour
{
    public Dialogue m_CheckDialogue;
    public GameObject m_Interactable;
    public SpriteRenderer m_BedSheetsLayer;
    public SpriteRenderer m_BedButtLayer;
    private bool m_Checked;
    
    public void TriggerEvent()
    {
        if(GameManager.instance.isTalking()) return;

        if(m_Checked || GameManager.instance.m_HappyLevel >= 10)
        {
            if(GameManager.instance.m_HappyLevel >= 10)
            {
                StartCoroutine(NGIO.UnlockMedal(72297));
            }
            else if(GameManager.instance.m_HappyLevel < 1)
            {
                StartCoroutine(NGIO.UnlockMedal(72298));
            }
            
            m_Interactable.SetActive(false);
            StartCoroutine(EndingEvent());
        }
        else 
        {
            StartCoroutine(TalkingEvent());
        }
    }

    IEnumerator TalkingEvent()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(m_CheckDialogue);
        while(GameManager.instance.isTalking())
        {
            yield return null;
        }
        m_Checked = true;
    }

    IEnumerator EndingEvent()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        player.DisableColliders(true);
        player.StopMovement(true);

        int newOrder = 5;
        m_BedSheetsLayer.sortingOrder = newOrder;
        m_BedButtLayer.sortingOrder = newOrder+1;
        
        Vector2 startPos = player.transform.position;
        Vector2 endPos = this.transform.position;
        float t = 0.0f;
        float movDur = Vector2.Distance(startPos, endPos) / 5.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / movDur;
            player.transform.position = Vector2.Lerp(startPos, endPos, t);
            player.m_ani.SetBool("isMoving", true);
            yield return null;
        }
        GameManager.instance.FadeOutBGM();
        player.m_ani.SetBool("isMoving", false);
        player.m_ani.SetBool("isSleeping", true);

        yield return new WaitForSeconds(0.5f);

        GameManager.instance.TransferPlayer(13, Vector2.zero, true);
    }
}
