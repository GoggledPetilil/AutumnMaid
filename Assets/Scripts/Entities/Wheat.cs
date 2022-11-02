using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheat : MonoBehaviour
{
    private Animator m_Anim;

    void Awake()
    {
        m_Anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            float oX = col.gameObject.transform.position.x;
            if(oX < this.transform.position.x)
            {
                m_Anim.SetTrigger("SwayLeft");
            }
            else 
            {
                m_Anim.SetTrigger("SwayRight");
            }
        }
    }
}
