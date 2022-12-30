using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class QuestGhost : MonoBehaviour
{
    public Dialogue[] m_StartDialogue;
    public Dialogue[] m_RepeatDialogue;
    public Dialogue m_ThanksDialogue;
    public GameObject m_PlushObject;
    [SerializeField] private Quest m_QuestData;
    [SerializeField] private Item m_ItemMemento;

    void Start()
    {
        m_PlushObject.SetActive(GameManager.instance.m_FlagGhostThanks);
    }

    public void TriggerDialogue()
    {
        Dialogue dialogue = null;
        GameManager.instance.AddQuest(m_QuestData);
        if(GameManager.instance.m_FlagMetGhost)
        {
            // Already met ghost
            if(GameManager.instance.m_FlagHaveMemento)
            {
                // Have the item
                if(GameManager.instance.m_FlagGhostThanks)
                {
                    dialogue = m_RepeatDialogue[1];
                }
                else 
                {
                    // Not given back yet
                    dialogue = m_ThanksDialogue;
                    GameManager.instance.m_FlagGhostThanks = true;
                    GameManager.instance.CompleteQuest(m_QuestData);
                    GameManager.instance.RemoveItem(m_ItemMemento);
                }
            }
            else 
            {
                dialogue = m_RepeatDialogue[0];
            }
        }
        else 
        {
            // Didn't meet ghost
            if(GameManager.instance.m_FlagHaveMemento)
            {
                dialogue = m_StartDialogue[1];
                GameManager.instance.m_FlagGhostThanks = true;
                GameManager.instance.CompleteQuest(m_QuestData);
                GameManager.instance.RemoveItem(m_ItemMemento);
            }
            else
            {
                dialogue = m_StartDialogue[0];
            }
            GameManager.instance.m_FlagMetGhost = true;
        }
        m_PlushObject.SetActive(GameManager.instance.m_FlagGhostThanks);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
