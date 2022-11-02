using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapleRoom : MonoBehaviour
{
    [SerializeField] private GameObject m_TrashHolder;
    [SerializeField] private GameObject m_ControlsHolder;
    [SerializeField] private Dialogue m_FinishDialogue;
    private bool m_CompletedQuest;
    
    void Start()
    {
        if(GameManager.instance.m_CleanedMapleRoom)
        {
            m_TrashHolder.SetActive(false);
            m_ControlsHolder.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if(m_CompletedQuest) return;
        
        GameObject[] trash = GameObject.FindGameObjectsWithTag("Destructable");

        if(trash.Length < 1)
        {
            // The player has cleaned everything.
            m_CompletedQuest = true;
            GameManager.instance.m_CleanedMapleRoom = true;
            m_ControlsHolder.SetActive(false);
            FindObjectOfType<DialogueManager>().StartDialogue(m_FinishDialogue);
        }
    }
}
