using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mint : MonoBehaviour
{
    public Dialogue m_IdleDialogue;
    public Dialogue m_UnlockDialogue;
    public Dialogue m_OpenDialogue;
    public Dialogue[] m_FinishDialogue;

    public void TriggerDialogue()
    {
        Dialogue dialogue = null;

        if(GameManager.instance.m_FlagThanksPostman)
        {
            if(GameManager.instance.m_DeliveryStage > 90)
            {
                dialogue = m_FinishDialogue[1];
            }
            else 
            {
                dialogue = m_FinishDialogue[0];
            }
        }
        else if(GameManager.instance.m_FlagMycenDoorOpen)
        {
            dialogue = m_OpenDialogue;
        }
        else if(GameManager.instance.m_FlagCheckedMycen)
        {
            dialogue = m_UnlockDialogue;
            GameManager.instance.m_FlagMycenDoorOpen = true;
        }
        else 
        {
            dialogue = m_IdleDialogue;
        }

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
