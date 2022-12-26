using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teacher : MonoBehaviour
{
    public Dialogue m_DeliveredDialogue;
    public Dialogue m_StandardDialogue;
    public Customer m_Customer;

    public void TriggerDialogue()
    {
        if(m_Customer.isCurrentCustomer())
        {
            m_Customer.TriggerDialogue();
            return;
        }
        
        Dialogue dialogue = null;
        if(GameManager.instance.m_DeliveryStage >= m_Customer.m_CustomerID)
        {
            dialogue = m_DeliveredDialogue;
        }
        else 
        {
            dialogue = m_StandardDialogue;
        }
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
