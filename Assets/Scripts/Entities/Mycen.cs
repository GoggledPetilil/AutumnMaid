using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mycen : MonoBehaviour
{
    public Dialogue m_StartDialogue;
    public Dialogue m_RepeatDialogue;
    public Dialogue m_ReturnDialogue;
    public Dialogue m_FinishDialogue;
    [SerializeField] private Item m_ToolboxItem;

    public void TriggerDialogue()
    {
        Dialogue dialogue = null;

        if(GameManager.instance.m_FlagReturnedToolbox)
        {
            dialogue = m_FinishDialogue;
        }
        else if(GameManager.instance.m_FlagThanksPostman)
        {
            dialogue = m_ReturnDialogue;
            GameManager.instance.RemoveItem(m_ToolboxItem);
            GameManager.instance.m_FlagReturnedToolbox = true;
        }
        else if(GameManager.instance.m_FlagAskedDIY)
        {
            dialogue = m_RepeatDialogue;
        }
        else 
        {
            dialogue = m_StartDialogue;
            GameManager.instance.m_FlagAskedDIY = true;
        }
        
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
