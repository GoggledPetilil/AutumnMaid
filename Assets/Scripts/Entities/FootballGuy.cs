using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballGuy : MonoBehaviour
{
    public Dialogue m_StartDialogue;
    public Dialogue m_CursedDialogue;
    public Dialogue[] m_FinishedDialogue;

    public void TriggerDialogue()
    {
        Dialogue dialogue = null;

        if(GameManager.instance.m_FlagGhostThanks)
        {
            if(GameManager.instance.m_FlagMetJan)
            {
                dialogue = m_FinishedDialogue[1];
            }
            else 
            {
                dialogue = m_FinishedDialogue[0];
                GameManager.instance.m_FlagMetJan = true;
            }
        }
        else if(GameManager.instance.m_FlagHaveMemento)
        {
            dialogue = m_CursedDialogue;
        }
        else 
        {
            dialogue = m_StartDialogue;
        }

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
