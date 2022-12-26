using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePizza : MonoBehaviour
{
    [SerializeField] private Customer m_Customer;
    [SerializeField] private GameObject m_SpriteHolder;
    [SerializeField] private GameObject m_InteractCircle;
    
    void Start()
    {
        int stage = GameManager.instance.m_DeliveryStage;
        if(m_Customer.isCurrentCustomer())
        {
            m_InteractCircle.SetActive(true);
        }
        else if((!GameManager.instance.m_IsDelivering && stage == m_Customer.m_CustomerID) || stage > m_Customer.m_CustomerID)
        {
            m_SpriteHolder.SetActive(true);
            m_InteractCircle.SetActive(false);
        }
        else 
        {
            m_SpriteHolder.SetActive(false);
            m_InteractCircle.SetActive(false);
        }
    }

    public void TriggerDialogue()
    {
        m_SpriteHolder.SetActive(true);
        if(m_Customer.isCurrentCustomer())
        {
            m_Customer.TriggerDialogue();
            return;
        }
    }
}
