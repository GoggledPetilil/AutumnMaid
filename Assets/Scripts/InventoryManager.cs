using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Item m_CurrentItem;
    public bool m_IsActive;
    private bool m_WaitInput;
    private bool m_IsPaused;

    [Header("Components")]
    [SerializeField] private CanvasGroup m_GroupHolder;
    [SerializeField] private CanvasGroup m_BlackTint;
    [SerializeField] private GameObject m_ItemHolder;
    [SerializeField] private Image m_ItemSR;
    [SerializeField] private Image m_PlayerSR;
    [SerializeField] private GameObject m_PlayerHolder;
    [SerializeField] private TMP_Text m_ItemGetText;

    [Header("Sprites")]
    [SerializeField] private Sprite m_OrbSprite;
    [SerializeField] private Sprite m_PlayerInitSprite;
    [SerializeField] private Sprite m_PlayerHappySprite;


    [Header("Audio Components")]
    [SerializeField] private AudioSource m_aud;
    [SerializeField] private AudioClip m_ItemGetSFX;
    [SerializeField] private AudioClip m_ItemRevealSFX;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        m_GroupHolder.alpha = 0.0f;
        m_IsActive = true;
    }

    void Update()
    {
        if(m_CurrentItem != null && m_IsActive)
        {
            StartEvent();
        }
        
        if(m_WaitInput)
        {
            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.K))
            {
                ExitGetAni();
            }
        }

        if(CanPause() && Input.GetKeyDown(KeyCode.Escape))
        {
            if(m_IsPaused)
            {
                Time.timeScale = 1.0f;
            }
            else 
            {
                Time.timeScale = 0.0f;
            }
        }
    }

    bool CanPause()
    {
        bool state = true;

        return state;
    }

    void PlayAudio(AudioClip clip)
    {
        m_aud.clip = clip;
        m_aud.Play();
    }

    public void GetItem(Item item)
    {
        if(m_WaitInput || !m_IsActive) return;

        m_CurrentItem = item;
    }

    void StartEvent()
    {
        m_IsActive = false;
        
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.StopMovement(true);

        StopAllCoroutines();
        StartCoroutine(ItemGetAnimation());
    }

    public void ExitGetAni()
    {
        if(!m_WaitInput) return;

        m_WaitInput = false;
        StartCoroutine(ExitEvent());
    }

    IEnumerator ItemGetAnimation()
    {
        // Initialize Everything
        RectTransform playerTransform = m_PlayerHolder.GetComponent<RectTransform>();
        RectTransform itemTransform = m_ItemHolder.GetComponent<RectTransform>();
        RectTransform itemSpriteTransform = m_ItemSR.gameObject.GetComponent<RectTransform>();
        RectTransform itemTextTransform = m_ItemGetText.GetComponent<RectTransform>();
        float playerStartPos = -512.0f;
        float orbStartPos = 640.0f;
        float t = 0.0f;

        m_PlayerSR.sprite = m_PlayerInitSprite;
        m_ItemSR.sprite = m_OrbSprite;
        playerTransform.anchoredPosition = new Vector2(playerTransform.anchoredPosition.x, playerStartPos);
        itemTransform.anchoredPosition = new Vector2(itemTransform.anchoredPosition.x, orbStartPos);
        playerTransform.localScale = Vector2.one;
        itemSpriteTransform.localScale = Vector2.one;
        itemTextTransform.localScale = Vector2.zero;

        m_GroupHolder.alpha = 1.0f;
        
        // Tint screen black
        float blackDur = 1.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / blackDur;
            m_BlackTint.alpha = t;
            yield return null;
        }

        // Flashing orb falls downwards
        t = 0.0f;
        float endItemY = -16.0f;
        float fallDur = 1.2f;
        PlayAudio(m_ItemGetSFX);
        while(t < 1.0f)
        {
            t += Time.deltaTime / fallDur;
            float y = Mathf.Lerp(orbStartPos, endItemY, t);
            itemTransform.anchoredPosition = new Vector2(itemTransform.anchoredPosition.x, y);
            yield return null;
        }

        // Flashing orb goes up slightly
        t = 0.0f;
        float endItemY2 = endItemY + 128.0f;
        float fallDur2 = 1.8f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / fallDur2;
            float y = Mathf.Lerp(endItemY, endItemY2, t);
            itemTransform.anchoredPosition = new Vector2(itemTransform.anchoredPosition.x, y);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        // Player moves upwards into the screen
        // Item goes into player's hands
        t = 0.0f;
        float endItemY3 = -380.0f;
        float endPlayY = 380.0f;
        float playRiseDur = 3.6f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / playRiseDur;
            float iY = Mathf.Lerp(endItemY2, endItemY3, t);
            float pY = Mathf.Lerp(playerStartPos, endPlayY, t);
            itemTransform.anchoredPosition = new Vector2(itemTransform.anchoredPosition.x, iY);
            playerTransform.anchoredPosition = new Vector2(playerTransform.anchoredPosition.x, pY);
            yield return null;
        }

        yield return new WaitForSeconds(0.32f);
        
        // Item orb shrinks
        t = 0.0f;
        float shrinkDur = 0.25f;
        PlayAudio(m_ItemRevealSFX);
        while(t < 1.0f)
        {
            t += Time.deltaTime / shrinkDur;
            float lerp = 1.0f - t;
            itemSpriteTransform.localScale = new Vector2(lerp, lerp);
            yield return null;
        }
        // Item sprite changes.
        m_ItemSR.sprite = m_CurrentItem.sprite;
        // Item orb comes back
        t = 0.0f;
        float multi = 0.75f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / shrinkDur;
            float lerp = Mathf.Lerp(0.0f, (1.0f + multi), t);
            itemSpriteTransform.localScale = new Vector2(lerp, lerp);
            yield return null;
        }
        t = 0.0f;
        float startItemScale = itemSpriteTransform.localScale.x;
        while(t < 1.0f)
        {
            t += Time.deltaTime / (shrinkDur * 1.5f);
            float lerp = Mathf.Lerp(startItemScale, 1.0f, t);
            itemSpriteTransform.localScale = new Vector2(lerp, lerp);
            yield return null;
        }

        // Player smiles 
        m_PlayerSR.sprite = m_PlayerHappySprite;
        float pScaleDur = 0.32f;
        float pScaleMulti = 0.1f;
        t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / pScaleDur;
            float lerpX = Mathf.Lerp(1.0f + pScaleMulti, 1.0f, t);
            float lerpY = Mathf.Lerp(1.0f - pScaleMulti, 1.0f, t);
            playerTransform.localScale = new Vector2(lerpX, lerpY);
            yield return null;
        }

        // "you got an item!!"
        float textTime = 0.32f;
        t = 0.0f;
        m_ItemGetText.text = "You obtained " + m_CurrentItem.itemName + "!";
        while(t < 1.0f)
        {
            t += Time.deltaTime / textTime;
            float lerp = Mathf.Lerp(0.0f, 1.0f, t);
            itemTextTransform.localScale = new Vector2(lerp, lerp);
            yield return null;
        }

        m_WaitInput = true;
    }

    IEnumerator ExitEvent()
    {
        float t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / 1.0f;
            m_GroupHolder.alpha = 1.0f - t;
            yield return null;
        }

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.StopMovement(false);
        m_CurrentItem = null;
        m_IsActive = true;
    }
}
