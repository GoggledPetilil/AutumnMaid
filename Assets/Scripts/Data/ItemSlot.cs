using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public string m_ItemName;
    public string m_ItemDescription;
    public Image m_Icon;
    public TMP_Text m_Amount;

    public void SetItemSlot(string name, string description, Sprite icon, int amount)
    {
        m_ItemName = name;
        m_ItemDescription = description;
        m_Icon.sprite = icon;
        if(amount < 1)
        {
            m_Amount.enabled = false;
        }
        else 
        {
            m_Amount.text = amount.ToString();
        }
    }

    public void SetItemDescription()
    {
        FindObjectOfType<PauseManager>().SetItemDescription(m_ItemName, m_ItemDescription);
    }

    public void ClearItemDescription()
    {
        FindObjectOfType<PauseManager>().ClearItemDescription();
    }
}
