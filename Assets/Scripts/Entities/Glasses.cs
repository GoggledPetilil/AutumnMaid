using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glasses : MonoBehaviour
{
    public Dialogue m_KnowDialogue;
    public Dialogue m_NoNoDialogue;
    [SerializeField] private Item m_ItemData;

    void Start()
    {
        if(GameManager.instance.m_FlagHaveGlasses)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void TriggerEvent()
    {
        StartCoroutine(ItemObtain());
    }

    IEnumerator ItemObtain()
    {
        GameManager.instance.m_FlagHaveGlasses = true;
        GameManager.instance.AddItem(m_ItemData);
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
}
