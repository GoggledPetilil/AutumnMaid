using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mailman : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject m_MailManHolder;
    [SerializeField] private GameObject m_MailManShadow;
    [SerializeField] private SpriteRenderer m_Renderer;
    [SerializeField] private Sprite m_IdleSprite;
    [SerializeField] private Sprite m_SadSprite;
    [SerializeField] private Animator m_ScooterAnim;
    [SerializeField] private ParticleSystem m_BrokenSmoke;
    [SerializeField] private ParticleSystem m_ScooterSmoke;
    [SerializeField] private Quest m_QuestData;

    [Header("Dialogue")]
    public Dialogue m_StartDialogue;
    public Dialogue m_RepeatDialogue;
    public Dialogue m_ThanksDialogue;
    public Dialogue m_OopsDialogue;
    
    void Start()
    {
        if(GameManager.instance.m_FlagThanksPostman)
        {
            m_ScooterAnim.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }

    public void TriggerDialogue()
    {
        if(GameManager.instance.m_FlagHaveToolbox)
        {
            StartCoroutine(EndEvent());
        }
        else 
        {
            StartCoroutine(DialogueEvent());
        }
    }

    IEnumerator DialogueEvent()
    {
        GameManager.instance.AddQuest(m_QuestData);
        m_Renderer.sprite = m_IdleSprite;

        if(GameManager.instance.m_FlagMetPostman)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(m_RepeatDialogue);
        }
        else 
        {
            GameManager.instance.m_FlagMetPostman = true;
            FindObjectOfType<DialogueManager>().StartDialogue(m_StartDialogue);
        }

        while(GameManager.instance.isTalking())
        {
            yield return null;
        }

        m_Renderer.sprite = m_SadSprite;
    }

    IEnumerator EndEvent()
    {
        GameManager.instance.m_FlagThanksPostman = true;
        m_Renderer.sprite = m_IdleSprite;

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        FindObjectOfType<DialogueManager>().StartDialogue(m_ThanksDialogue);

        while(GameManager.instance.isTalking())
        {
            yield return null;
        }
        player.StopMovement(true);
        m_BrokenSmoke.Stop();

        Vector2 startPos = this.transform.position;
        Vector2 endPos = m_ScooterAnim.transform.position;
        float t = 0.0f;
        float movDur = 0.5f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / movDur;
            this.transform.position = Vector2.Lerp(startPos, endPos, t);
            m_MailManShadow.transform.localPosition = this.transform.localPosition;
            yield return null;
        }

        m_Renderer.enabled = false;
        m_MailManShadow.SetActive(false);
        m_ScooterAnim.SetBool("isDriving", true);
        m_ScooterSmoke.Play();

        yield return new WaitForSeconds(0.5f);

        Vector2 scooterStart = m_ScooterAnim.transform.position;
        Vector2 scooterEnd = new Vector2(scooterStart.x, scooterStart.y + 12.0f);
        t = 0.0f;
        float driveDur = 2.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / driveDur;
            m_ScooterAnim.transform.position = Vector2.Lerp(scooterStart, scooterEnd, t);
            yield return null;
        }
        m_ScooterAnim.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.0f);
        FindObjectOfType<DialogueManager>().StartDialogue(m_OopsDialogue);

        while(GameManager.instance.isTalking())
        {
            yield return null;
        }

        GameManager.instance.IncreaseHappiness();
        GameManager.instance.CompleteQuest(m_QuestData);
        this.gameObject.SetActive(false);
    }
}
