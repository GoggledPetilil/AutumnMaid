using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFarmerManager : MonoBehaviour
{
    public Dialogue m_FirstTalkDialogue;
    public Dialogue m_RepeatDialogue;
    public Dialogue m_ThanksDialogue;
    public Dialogue m_ThanksRepeatDialogue;
    
    public void TriggerDialogue()
    {
        Dialogue dialogue = null;
        if(GameManager.instance.m_FlagMetFarmer == false)
        {
            dialogue = m_FirstTalkDialogue;
            GameManager.instance.m_FlagMetFarmer = true;
        }
        else 
        {
            if(GameManager.instance.m_SheepSaved.Count >= 3)
            {
                if(GameManager.instance.m_FlagSavedSheep)
                {
                    dialogue = m_ThanksRepeatDialogue;
                }
                else
                {
                    dialogue = m_ThanksDialogue;
                    GameManager.instance.m_FlagSavedSheep = true;
                }
            }
            else 
            {
                dialogue = m_RepeatDialogue;
            }
        }
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
