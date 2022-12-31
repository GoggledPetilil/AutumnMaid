using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public string m_ItemName;
    public Image m_Icon;
    public TMP_Text m_Amount;

    public void SetItemSlot(string name, Sprite icon, int amount)
    {
        m_ItemName = name;
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
}
