using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Entity
{
    [Header("Cat Components")]
    [SerializeField] private Animator m_Anim;
    [SerializeField] private Dialogue m_NoFoodDialogue;
    [SerializeField] private Dialogue m_FeedingDialogue;
    [SerializeField] private SpriteRenderer m_sr;
    [SerializeField] private Quest m_QuestData;

    void Start()
    {
        if(GameManager.instance.m_FlagFedCat)
        {
            m_Anim.SetBool("isContent", true);
        }
    }
    
    public void TriggerEvent()
    {
        if(!GameManager.instance.m_FlagFedCat)
        {
            Dialogue dialogue = null;
            if(GameManager.instance.m_FishAmount > 0)
            {
                dialogue = m_FeedingDialogue;
                GameManager.instance.m_FlagFedCat = true;
                m_Anim.SetBool("isHappy", true);
                GameManager.instance.CompleteQuest(m_QuestData);
                GameManager.instance.m_FishAmount -= 1;
            }
            else 
            {
                dialogue = m_NoFoodDialogue;
                GameManager.instance.AddQuest(m_QuestData);
            }
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        }
        else 
        {
            Player p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            p.PatObject(this);
        }
    }
}
