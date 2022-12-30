using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Entity
{
    [Header("Cat Components")]
    [SerializeField] private Dialogue m_NoFoodDialogue;
    [SerializeField] private Dialogue m_FeedingDialogue;
    [SerializeField] private SpriteRenderer m_sr;
    [SerializeField] private Sprite[] m_CatSprites;
    [SerializeField] private Quest m_QuestData;

    void Start()
    {
        if(GameManager.instance.m_FlagFedCat)
        {
            m_sr.sprite = m_CatSprites[2];
        }
        else 
        {
            m_sr.sprite = m_CatSprites[1];
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
                GameManager.instance.CompleteQuest(m_QuestData);
                m_sr.sprite = m_CatSprites[0];
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
