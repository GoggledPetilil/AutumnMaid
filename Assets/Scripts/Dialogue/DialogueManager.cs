using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Components")]
    public TMP_Text m_NameText;
    public TMP_Text m_DialogueText;
    public GameObject m_AdvanceArrow;

    [Header("Components")]
    [SerializeField] private Animator m_BoxAnim;
    [SerializeField] private Animator m_PortraitAnim;
    [SerializeField] private AudioSource m_Aud;
    [SerializeField] private AudioClip m_DefaultTextSFX;
    [SerializeField] private CanvasGroup m_Canvas;
    [SerializeField] private Image m_PortraitImage;

    [Header("Parameters")]
    public float m_TextSpeed;
    public Queue<Dialogue.Info> m_DialogueInfo = new Queue<Dialogue.Info>();
    private bool m_IncreaseHappy;
    private string m_CurrentSentence;
    private bool m_InConversation;
    private bool m_IsTyping;
    private float talkTimer; // Prevents Player from insta-starting another conversation after finishing.
    private float talkDelay = 0.24f;


    void Update()
    {
        if(m_InConversation && Time.time > talkTimer)
        {
            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.K))
            {
                if(m_IsTyping)
                {
                    StopAllCoroutines();
                    m_DialogueText.text = m_CurrentSentence;
                    m_IsTyping = false;
                    m_AdvanceArrow.SetActive(true);
                    m_PortraitAnim.SetBool("isTalking", false);
                }
                else 
                {
                    DisplayNextSentence();
                }
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if(m_InConversation || Time.time < talkTimer) return;

        // Enqueue Dialogue
        m_DialogueInfo.Clear();
        foreach(Dialogue.Info info in dialogue.dialogueInfo)
        {
            if(info.sentence != "")
            {
                m_DialogueInfo.Enqueue(info);
            }
        }

        // See if there's a reward
        m_IncreaseHappy = dialogue.increasesHappiness;

        // Entering Dialogue Mode.
        m_Canvas.alpha = 1.0f;
        talkTimer = Time.time + talkDelay;
        InventoryManager.instance.m_IsActive = false;
        m_BoxAnim.SetBool("isOpen", true);
        m_PortraitAnim.SetBool("isOpen", true);
        m_InConversation = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Player>().StopMovement(true);

        // Immediatly Dequeue the first sentence.
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // If there is no next sentence, end dialogue.
        if(m_DialogueInfo.Count < 1)
        {
            EndDialogue();
            return;
        }
        
        m_AdvanceArrow.SetActive(false);
        
        // Get current information.
        Dialogue.Info info = m_DialogueInfo.Dequeue();

        // Display the character
        if(info.character != null)
        {
            m_NameText.text = info.character.charName;
            m_PortraitImage.sprite = info.character.portraitSprite;
            m_Aud.clip = info.character.voiceClip;
        }
        else 
        {
            m_NameText.text = "";
            m_Aud.clip = m_DefaultTextSFX;
        }
        m_PortraitImage.enabled = info.character != null;

        // Start typing out the sentence
        m_CurrentSentence = info.sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(m_CurrentSentence));
    }

    public void EndDialogue()
    {        
        m_BoxAnim.SetBool("isOpen", false);
        m_PortraitAnim.SetBool("isOpen", false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Player>().StopMovement(false);
        InventoryManager.instance.m_IsActive = true;

        m_InConversation = false;
        talkTimer = Time.time + talkDelay;
        
        if(m_IncreaseHappy == true)
        {
            GameManager.instance.IncreaseHappiness();
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        m_IsTyping = true;
        m_PortraitAnim.SetBool("isTalking", true);
        m_DialogueText.text = "";
        for(int i = 0; i < sentence.ToCharArray().Length + 1; i++)
        {
            m_DialogueText.text = sentence.Substring(0, i);
            m_DialogueText.text += "<color=#00000000>" + sentence.Substring(i) + "</color>";
            
            m_Aud.Play();
            yield return new WaitForSeconds(Mathf.Clamp(1.0f - m_TextSpeed, 0.01f, 1.0f));
        }
        m_IsTyping = false;
        m_AdvanceArrow.SetActive(true);
        m_PortraitAnim.SetBool("isTalking", false);
    }
}
