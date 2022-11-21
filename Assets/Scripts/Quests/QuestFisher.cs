using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFisher : MonoBehaviour
{
    public Dialogue m_FirstTalkDialogue;
    public Dialogue m_RepeatDialogue;
    public Dialogue m_RepeatDialogue_1;
    public Dialogue m_ThanksDialogue;
    public Dialogue m_ThanksRepeatDialogue;

    public void TriggerDialogue()
    {
        Dialogue dialogue = null;
        if(GameManager.instance.m_FlagMetFisher == false)
        {
            dialogue = m_FirstTalkDialogue;
            GameManager.instance.m_FlagMetFisher = true;
        }
        else 
        {
            if(GameManager.instance.m_FishAmount >= 4)
            {
                if(GameManager.instance.m_FlagThankFisher)
                {
                    dialogue = m_ThanksRepeatDialogue;
                }
                else
                {
                    dialogue = m_ThanksDialogue;
                    GameManager.instance.m_FlagThankFisher = true;
                }
            }
            else if(GameManager.instance.m_FishAmount < 1)
            {
                dialogue = m_RepeatDialogue;
            }
            else 
            {
                dialogue = m_RepeatDialogue_1;
            }
        }
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
