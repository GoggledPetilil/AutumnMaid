using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doggie : Entity
{
    [Header("Dog Components")]
    [SerializeField] private Dialogue m_StealDialogue;
    [SerializeField] private SpriteRenderer m_sr;
    [SerializeField] private Sprite[] m_DogSprites;

    void Start()
    {
        if(GameManager.instance.m_FlagHaveMemento)
        {
            m_sr.sprite = m_DogSprites[1];
        }
        else 
        {
            m_sr.sprite = m_DogSprites[0];
        }
    }

    public void TriggerEvent()
    {
        if(GameManager.instance.m_FlagMetGhost && !GameManager.instance.m_FlagHaveMemento)
        {
            m_sr.sprite = m_DogSprites[1];
            FindObjectOfType<DialogueManager>().StartDialogue(m_StealDialogue);
            GameManager.instance.m_FlagHaveMemento = true;
        }
        else 
        {
            Player p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            p.PatObject(this);
        }
    }
}
