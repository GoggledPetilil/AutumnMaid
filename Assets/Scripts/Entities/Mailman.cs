using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mailman : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject m_MailManHolder;
    [SerializeField] private SpriteRenderer m_Renderer;
    [SerializeField] private Sprite m_IdleSprite;
    [SerializeField] private Sprite m_SadSprite;
    [SerializeField] private Animator m_ScooterAnim;
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
            m_MailManHolder.SetActive(false);
        }
    }

    public void TriggerDialogue()
    {
        m_Renderer.sprite = m_IdleSprite;
        if(GameManager.instance.m_FlagHaveToolbox)
        {
            StartCoroutine(EndEvent());
        }
        else if(GameManager.instance.m_FlagMetPostman)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(m_RepeatDialogue);
        }
        else 
        {
            GameManager.instance.m_FlagMetPostman = true;
            FindObjectOfType<DialogueManager>().StartDialogue(m_StartDialogue);
        }
    }

    IEnumerator EndEvent()
    {
        GameManager.instance.m_FlagThanksPostman = true;
        FindObjectOfType<DialogueManager>().StartDialogue(m_ThanksDialogue);

        while(GameManager.instance.isTalking())
        {
            yield return null;
        }

        Vector2 startPos = this.transform.position;
        Vector2 endPos = m_ScooterAnim.transform.position;
        float t = 0.0f;
        float movDur = 1.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / movDur;
            this.transform.position = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        m_Renderer.enabled = false;
        m_ScooterAnim.SetBool("isDriving", true);

        yield return new WaitForSeconds(0.2f);

        Vector2 scooterStart = m_ScooterAnim.transform.position;
        Vector2 scooterEnd = new Vector2(scooterStart.x, scooterStart.y + 12.0f);
        t = 0.0f;
        float driveDur = 1.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / driveDur;
            m_ScooterAnim.transform.position = Vector2.Lerp(scooterStart, scooterEnd, t);
            yield return null;
        }
        m_ScooterAnim.gameObject.SetActive(false);

        GameManager.instance.IncreaseHappiness();

        yield return new WaitForSeconds(1.5f);

        FindObjectOfType<DialogueManager>().StartDialogue(m_OopsDialogue);
    }
}
