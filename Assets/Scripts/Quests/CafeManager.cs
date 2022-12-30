using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeManager : MonoBehaviour
{
    private int m_QuestQuota = 7;

    [Header("Dialogue")]
    [SerializeField] private Dialogue[] m_StartDialogue;
    [SerializeField] private Dialogue[] m_RepeatDialogue;
    [SerializeField] private Dialogue[] m_WinDialogue;
    [SerializeField] private Dialogue[] m_ThanksDialogue;
    [SerializeField] private Dialogue[] m_TeaDialogue;
    private bool canTalk;
    
    [Header("Sprite Components")]
    [SerializeField] private SpriteRenderer m_NikoRenderer;
    [SerializeField] private Sprite m_SpriteIdle;
    [SerializeField] private Sprite m_SpriteSad;

    [Header("Quest Data")]
    [SerializeField] private Quest m_QuestData;
    [SerializeField] private Quest m_TeaQuest;
    [SerializeField] private Item m_TeaItem;

    void Start()
    {
        if(GameManager.instance.m_DeliveryStage > 90)
        {
            m_NikoRenderer.sprite = m_SpriteIdle;
        }
        else 
        {
            m_NikoRenderer.sprite = m_SpriteSad;
        }
        
        if(!GameManager.instance.m_FlagMetCafe)
        {
            GameManager.instance.m_DeliveryStage = -1;
            GameManager.instance.m_FlagMetCafe = true; 
        }

        canTalk = true;
    }

    public void TriggerDialogue()
    {
        if(canTalk == false) return;
        GameManager.instance.AddQuest(m_QuestData);
        
        if(GameManager.instance.m_IsDelivering)
        {
            // Repeat dialogue
            Dialogue dialogue = null;
            int i = Mathf.Clamp(GameManager.instance.m_DeliveryStage, 0, m_RepeatDialogue.Length);
            dialogue = m_RepeatDialogue[i];
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        }
        else if(GameManager.instance.m_DeliveryStage > 90)
        {
            // Dialogue for after the quest.
            Dialogue dialogue = null;
            
            // Check wether the player has done enough quests to get tea
            if(GameManager.instance.m_FlagHaveTea)
            {
                if(GameManager.instance.m_FlagDrankTea)
                {
                    // Tea drank
                    dialogue = m_TeaDialogue[1];
                }
                else
                {
                    // Tea already obtained
                    dialogue = m_TeaDialogue[0];
                }
            }
            else 
            {
                if(GameManager.instance.m_HappyLevel < m_QuestQuota)
                {
                    // Tea isn't ready yet
                    dialogue = m_ThanksDialogue[0];
                }
                else
                {
                    // Tea is ready
                    dialogue = m_ThanksDialogue[1];
                    GameManager.instance.m_FlagHaveTea = true;
                    GameManager.instance.AddQuest(m_TeaQuest);
                    GameManager.instance.AddItem(m_TeaItem);
                }
            }

            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        }
        else 
        {
            // Do the next delivery!
            StartCoroutine(DialogueEvent());
        }
    }

    IEnumerator DialogueEvent()
    {
        canTalk = false;
        // Show dialogue
        GameManager.instance.m_DeliveryStage++;
        Dialogue dialogue = null;

        int i = Mathf.Clamp(GameManager.instance.m_DeliveryStage, 0, m_StartDialogue.Length);
        if(GameManager.instance.m_DeliveryStage >= m_StartDialogue.Length)
        {
            m_NikoRenderer.sprite = m_SpriteIdle;
            if(GameManager.instance.m_HappyLevel < m_QuestQuota)
            {
                dialogue = m_WinDialogue[0];
            }
            else 
            {
                dialogue = m_WinDialogue[1];
                GameManager.instance.m_FlagHaveTea = true;
                GameManager.instance.AddQuest(m_TeaQuest);
                GameManager.instance.AddItem(m_TeaItem);
            }
        }
        else 
        {
            dialogue = m_StartDialogue[i];
        }

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        while(GameManager.instance.isTalking())
        {
            yield return null;
        }
        
        if(GameManager.instance.m_DeliveryStage >= m_StartDialogue.Length)
        {
            GameManager.instance.m_DeliveryStage = 99;
            GameManager.instance.IncreaseHappiness();
            GameManager.instance.CompleteQuest(m_QuestData);
        }
        else 
        {
            GameManager.instance.StartDelivery(true);
        }

        canTalk = true;
    }
}
