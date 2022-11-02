using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapleDoor : DoorEvent
{
    public Dialogue m_BlockDialogue;

    public void TriggerMapleDoor()
    {
        GameObject[] trash = GameObject.FindGameObjectsWithTag("Destructable");

        if(trash.Length > 0)
        {
            // There is still trash left to be cleaned.
            FindObjectOfType<DialogueManager>().StartDialogue(m_BlockDialogue);
        }
        else
        {
            // The player has cleaned everything.
            base.TriggerDoor();
        }
    }
}
