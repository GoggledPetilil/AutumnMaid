using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MycenDoor : DoorEvent
{
    public Dialogue[] m_BlockDialogue;

    public override void TriggerDoor()
    {
        if(!GameManager.instance.m_FlagMycenDoorOpen)
        {
            // The door is locked
            if(GameManager.instance.m_FlagMetPostman)
            {
                FindObjectOfType<DialogueManager>().StartDialogue(m_BlockDialogue[1]);
            }
            else 
            {
                FindObjectOfType<DialogueManager>().StartDialogue(m_BlockDialogue[0]);
            }
            GameManager.instance.m_FlagCheckedMycen = true;
        }
        else
        {
            // The player can enter.
            base.TriggerDoor();
        }
    }
}
