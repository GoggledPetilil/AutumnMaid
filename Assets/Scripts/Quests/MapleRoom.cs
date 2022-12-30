using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapleRoom : MonoBehaviour
{
    [SerializeField] private GameObject m_TrashHolder;
    [SerializeField] private GameObject m_ControlsHolder;
    [SerializeField] private Dialogue m_FinishDialogue;
    [SerializeField] private Quest m_QuestData;
    
    void Start()
    {
        if(GameManager.instance.m_FlagCleanedMapleRoom)
        {
            m_TrashHolder.SetActive(false);
            m_ControlsHolder.SetActive(false);
            this.gameObject.SetActive(false);
        }
        else 
        {
            GameManager.instance.AddQuest(m_QuestData);
        }
    }

    void Update()
    {
        if(GameManager.instance.m_FlagCleanedMapleRoom) return;
        
        GameObject[] trash = GameObject.FindGameObjectsWithTag("Destructable");

        if(trash.Length < 1)
        {
            // The player has cleaned everything.
            GameManager.instance.CompleteQuest(m_QuestData);
            GameManager.instance.m_FlagCleanedMapleRoom = true;
            m_ControlsHolder.SetActive(false);
            FindObjectOfType<DialogueManager>().StartDialogue(m_FinishDialogue);
        }
    }
}
