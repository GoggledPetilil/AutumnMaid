using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGrandma : MonoBehaviour
{
    public Dialogue m_FirstTalkDialogue;
    public Dialogue m_RepeatDialogue;
    public Dialogue m_ThanksDialogue;
    public Dialogue m_ThanksRepeatDialogue;
    public Dialogue m_AlreadyGotDialogue;

    public void TriggerDialogue()
    {
        Dialogue dialogue = null;
        if(GameManager.instance.m_FlagLadyHelped)
        {
            dialogue = m_ThanksRepeatDialogue;
        }
        if(GameManager.instance.m_FlagHaveGlasses)
        {
            if(GameManager.instance.m_FlagMetOldLady)
            {
                dialogue = m_ThanksDialogue;
            }
            else 
            {
                dialogue = m_AlreadyGotDialogue;
            }
            GameManager.instance.m_FlagLadyHelped = true;
        }
        else 
        {
            if(GameManager.instance.m_FlagMetOldLady)
            {
                dialogue = m_RepeatDialogue;
            }
            else 
            {
                dialogue = m_FirstTalkDialogue;
                GameManager.instance.m_FlagMetOldLady = true;
            }
        }
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
