using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelenaRoom : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject m_TrashHolder;
    [SerializeField] private GameObject m_Book;
    [SerializeField] private Helena m_Helena;
    private Vector2 m_HelenaSitPos = new Vector2(-4.5f, 0.5f);

    [Header("Dialogue")]
    public Dialogue m_FindBookDialogue;
    public Dialogue m_ReadingDialogue;
    public Dialogue m_SleepingDialogue;

    void Start()
    {
        m_TrashHolder.SetActive(!GameManager.instance.m_FlagCleanedHelenaRoom);
        m_Book.SetActive(GameManager.instance.m_FlagCleanedHelenaRoom);

        if(GameManager.instance.m_FlagHelenaBook)
        {
            m_Helena.transform.position = m_HelenaSitPos;
            m_Helena.FallAsleep();
            m_Book.SetActive(false);
        }
    }

    void Update()
    {
        if(GameManager.instance.m_FlagCleanedHelenaRoom) return;
        
        GameObject[] trash = GameObject.FindGameObjectsWithTag("Destructable");

        if(trash.Length < 1)
        {
            // The player has cleaned everything.
            GameManager.instance.m_FlagCleanedHelenaRoom = true;
            m_TrashHolder.SetActive(false);
            m_Book.SetActive(true);
        }
    }

    public void TriggerEvent()
    {
        StartCoroutine(BookEvent());
    }

    IEnumerator BookEvent()
    {
        GameManager.instance.m_FlagHelenaBook = true;
        m_Book.SetActive(false);
        m_Helena.CanInteract(false);
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        float dialogueDelay = 0.25f;
        
        FindObjectOfType<DialogueManager>().StartDialogue(m_FindBookDialogue);
        while(GameManager.instance.isTalking())
        {
            yield return null;
        }

        // Sit on couch
        player.DisableColliders(true);
        Vector2 playerStart = player.transform.position;
        Vector2 playerEnd = new Vector2(-2.0f, 0.3f);
        Vector2 helenaStart = m_Helena.transform.position;
        Vector2 helenaEnd = m_HelenaSitPos;
        float movDur = Vector2.Distance(playerStart, playerEnd) / 5.0f;
        float t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / movDur;
            player.transform.position = Vector2.Lerp(playerStart, playerEnd, t);
            m_Helena.transform.position = Vector2.Lerp(helenaStart, helenaEnd, t);
            player.m_ani.SetBool("isMoving", true);
            yield return null;
        }
        player.m_ani.SetBool("isMoving", false);
        player.m_ani.SetBool("isReading", true);
        m_Helena.SitDown();
        
        yield return new WaitForSeconds(dialogueDelay);

        FindObjectOfType<DialogueManager>().StartDialogue(m_ReadingDialogue);
        while(GameManager.instance.isTalking())
        {
            yield return null;
        }

        yield return new WaitForSeconds(dialogueDelay);
        m_Helena.FallAsleep();

        FindObjectOfType<DialogueManager>().StartDialogue(m_SleepingDialogue);
        while(GameManager.instance.isTalking())
        {
            yield return null;
        }

        // Get off
        player.m_ani.SetBool("isReading", false);
        t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / movDur;
            player.transform.position = Vector2.Lerp(playerEnd, playerStart, t);
            player.m_ani.SetBool("isMoving", true);
            yield return null;
        }
        player.m_ani.SetBool("isMoving", false);
        player.DisableColliders(false);

        GameManager.instance.IncreaseHappiness();
        m_Helena.CanInteract(true);
    }
}
