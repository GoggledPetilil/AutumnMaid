using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oswald : MonoBehaviour
{
    public Dialogue m_SheepDialogue;
    public Dialogue[] m_FishDialogue;

    public void TriggerDialogue()
    {
        Dialogue dialogue = null;

        if(GameManager.instance.m_FlagMetOswald)
        {
            dialogue = m_FishDialogue[1];
        }
        else if(GameManager.instance.m_FlagThankFisher)
        {
            dialogue = m_FishDialogue[0];
            GameManager.instance.m_FlagMetOswald = true;
        }
        else
        {
            dialogue = m_SheepDialogue;
        }

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
