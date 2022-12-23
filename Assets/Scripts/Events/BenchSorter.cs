using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchSorter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_BenchSprite;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            m_BenchSprite.sortingOrder = 1;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            m_BenchSprite.sortingOrder = 0;
        }
    }
}
