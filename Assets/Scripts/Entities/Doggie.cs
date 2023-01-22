using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doggie : Entity
{
    [Header("Dog Components")]
    [SerializeField] private Dialogue m_StealDialogue;
    [SerializeField] private SpriteRenderer m_sr;
    [SerializeField] private Sprite[] m_DogSprites;
    [SerializeField] private Sprite m_DogSadSprites;
    [SerializeField] private Item m_ItemMemento;
    [SerializeField] private ParticleSystem m_SleepParticles;
    private bool isTalking;

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
        if(isTalking) return;
        if(GameManager.instance.m_FlagMetGhost && !GameManager.instance.m_FlagHaveMemento)
        {
            StartCoroutine(DialogueEvent());
        }
        else 
        {
            Player p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            p.PatObject(this);
        }
    }

    IEnumerator DialogueEvent()
    {
        isTalking = true;
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.StopMovement(true);

        m_sr.sprite = m_DogSadSprites;
        m_SleepParticles.Stop();
        FindObjectOfType<DialogueManager>().StartDialogue(m_StealDialogue);
        GameManager.instance.m_FlagHaveMemento = true;
        GameManager.instance.AddItem(m_ItemMemento);

        while(GameManager.instance.isTalking())
        {
            yield return null;
        }

        player.StopMovement(false);
        m_sr.sprite = m_DogSprites[1];
        isTalking = false;
    }
}
