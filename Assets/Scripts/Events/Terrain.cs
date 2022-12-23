using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [SerializeField] private Player.TerrainTag m_Terrain;
    [SerializeField] private Player.TerrainTag m_ExitTerrain;
    
    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Player p = other.gameObject.GetComponent<Player>();
            p.m_CurrentTerrain = m_Terrain;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Player p = other.gameObject.GetComponent<Player>();
            p.m_CurrentTerrain = m_ExitTerrain;
        }
    }
}
