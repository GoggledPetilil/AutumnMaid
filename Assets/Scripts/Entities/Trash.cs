using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Destructable
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            player.m_SpeedMod = 0.5f;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            player.m_SpeedMod = 1.0f;
        }
    }
}
