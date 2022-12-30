using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helena : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator m_Anim;
    [SerializeField] private GameObject m_InteractCircle;
    [SerializeField] private Quest m_QuestData;

    [Header("Dialogue")]
    public Dialogue m_StartDialogue;
    public Dialogue m_RepeatDialogue;
    public Dialogue m_SleepDialogue;

    public void SitDown()
    {
        m_Anim.SetBool("isSitting", true);
    }

    public void FallAsleep()
    {
        m_Anim.SetBool("isSleeping", true);
    }

    public Quest GetQuestData()
    {
        return m_QuestData;
    }

    public void CanInteract(bool state)
    {
        m_InteractCircle.SetActive(state);
    }

    public void TriggerDialogue()
    {
        GameManager.instance.AddQuest(m_QuestData);
        Dialogue dialogue = null;
        
        if(GameManager.instance.m_FlagHelenaBook)
        {
            dialogue = m_SleepDialogue;
        }
        else if(GameManager.instance.m_FlagMetHelena)
        {
            dialogue = m_RepeatDialogue;
        }
        else 
        {
            dialogue = m_StartDialogue;
            GameManager.instance.m_FlagMetHelena = true;
        }

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
