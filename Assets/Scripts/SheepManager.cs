using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour
{
    [SerializeField] private GameObject[] m_AllSheep;

    void Start()
    {
        // IF player has Sheep on hand
        if(GameManager.instance.m_SheepOnHand.Count > 0)
        {
            for(int i = 0; i < GameManager.instance.m_SheepOnHand.Count; i++)
            {
                GameManager.instance.SaveSheep(GameManager.instance.m_SheepOnHand[i]);
            }
            GameManager.instance.m_SheepOnHand.Clear();
        }

        // Show the saved sheep:
        if(GameManager.instance.m_SheepSaved.Count < m_AllSheep.Length)
        {
            for(int i = 0; i < m_AllSheep.Length; i++)
            {
                if(GameManager.instance.m_SheepSaved.Contains(i))
                {
                    m_AllSheep[i].SetActive(true);
                }
                else 
                {
                    m_AllSheep[i].SetActive(false);
                }
            }
        }
    }
}
