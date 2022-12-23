using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTrigger : MonoBehaviour
{
    [SerializeField] private Player.TerrainTag m_NorthTerrain;
    [SerializeField] private Player.TerrainTag m_SouthTerrain;

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Player p = other.gameObject.GetComponent<Player>();
            
            Player.TerrainTag newTerrain = m_NorthTerrain;
            if(other.transform.position.y < this.transform.position.y)
            {
                newTerrain = m_SouthTerrain;
            }

            p.m_CurrentTerrain = newTerrain;
        }
    }
}
