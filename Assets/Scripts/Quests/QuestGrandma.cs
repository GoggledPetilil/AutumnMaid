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

    public Customer m_Customer;

    [SerializeField] private SpriteRenderer m_Renderer;
    [SerializeField] private Sprite m_GlassesSprite;
    [SerializeField] private Sprite m_NoGlassesSprite;

    void Start()
    {
        if(GameManager.instance.m_FlagLadyHelped)
        {
            m_Renderer.sprite = m_GlassesSprite;
        }
        else 
        {
            m_Renderer.sprite = m_NoGlassesSprite;
        }
    }

    public void TriggerDialogue()
    {
        if(m_Customer.isCurrentCustomer())
        {
            m_Customer.TriggerDialogue();
            return;
        }
        
        Dialogue dialogue = null;
        if(GameManager.instance.m_FlagLadyHelped)
        {
            dialogue = m_ThanksRepeatDialogue;
        }
        else if(GameManager.instance.m_FlagHaveGlasses)
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
            m_Renderer.sprite = m_GlassesSprite;
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
