using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool m_WillTurn;
    public UnityEvent m_Event;
    
    public void InvokeEvent()
    {
        m_Event.Invoke();
    }

    /*void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {

        }
    }*/

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            if(player.m_InteractEvent == null)
            {
                player.m_InteractEvent = this;
            }
            if(!GameManager.instance.isTalking())
            {
                player.PromptAppear(true);
            }
            
            if(m_WillTurn && gameObject.transform.parent.transform.CompareTag("NPC"))
            {
                if(col.gameObject.transform.position.x > gameObject.transform.parent.transform.position.x)
                {
                    Entity e = gameObject.transform.parent.GetComponent(typeof(Entity)) as Entity;
                    if(e == null) return;
                    e.FlipSprite(false);
                }
                else if(col.gameObject.transform.position.x < gameObject.transform.parent.transform.position.x)
                {
                    Entity e = gameObject.transform.parent.GetComponent(typeof(Entity)) as Entity;
                    if(e == null) return;
                    e.FlipSprite(true);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            if(player.m_InteractEvent == this)
            {
                player.m_InteractEvent = null;
                player.PromptAppear(false);
            }
        }
    }
}
