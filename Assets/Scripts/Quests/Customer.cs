using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public int m_CustomerID;
    public Dialogue m_DeliveryDialogue;

    public void TriggerDialogue()
    {
        if(!GameManager.instance.m_IsDelivering || GameManager.instance.m_DeliveryStage != m_CustomerID) return;

        FindObjectOfType<DialogueManager>().StartDialogue(m_DeliveryDialogue);
        GameManager.instance.StartDelivery(false);
    }

    public bool isCurrentCustomer()
    {
        bool isCurrent = false;
        if(GameManager.instance.m_IsDelivering && GameManager.instance.m_DeliveryStage == m_CustomerID)
        {
            isCurrent = true;
        }

        return isCurrent;
    }
}
