using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glasses : MonoBehaviour
{
    public Dialogue m_KnowDialogue;
    public Dialogue m_NoNoDialogue;

    void Start()
    {
        if(GameManager.instance.m_FlagHaveGlasses)
        {
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator ItemObtain()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        
        if(GameManager.instance.m_FlagMetOldLady)
        {
            dm.StartDialogue(m_KnowDialogue);
        }
        else 
        {
            dm.StartDialogue(m_NoNoDialogue);
        }

        while(dm.isTalking())
        {
            yield return null;
        }

        this.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            GameManager.instance.m_FlagHaveGlasses = true;
            StartCoroutine(ItemObtain());
        }
    }
}
