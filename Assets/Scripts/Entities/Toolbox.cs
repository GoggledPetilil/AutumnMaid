using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbox : MonoBehaviour
{
    public Dialogue m_NormalDialogue;
    public Dialogue m_NoPermissionDialogue;
    public Dialogue m_TakeDialogue;

    void Start()
    {
        if(GameManager.instance.m_FlagHaveToolbox)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void TriggerEvent()
    {
        Dialogue dialogue = null;
        if(GameManager.instance.m_FlagAskedDIY)
        {
            dialogue = m_TakeDialogue;

            GameManager.instance.m_FlagHaveToolbox = true;
            this.gameObject.SetActive(false);
        }
        else if(GameManager.instance.m_FlagMetPostman)
        {
            dialogue = m_NoPermissionDialogue;
        }
        else 
        {
            dialogue = m_NormalDialogue;
        }
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
