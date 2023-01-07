using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_CanvasTransform;
    [SerializeField] private GameObject m_MapButton;
    [SerializeField] private GameObject m_ItemButton;
    [SerializeField] private GameObject m_SystemButton;
    [SerializeField] private GameObject m_MapHolder;
    [SerializeField] private RectTransform m_IconTransform;
    [SerializeField] private GameObject m_ItemHolder;
    [SerializeField] private Transform m_InventoryHolder;
    [SerializeField] private TMP_Text m_QuestLog;
    [SerializeField] private TMP_Text m_ItemName;
    [SerializeField] private TMP_Text m_ItemDescription;
    [SerializeField] private Item m_FishItem;
    [SerializeField] private GameObject m_SystemHolder;
    [SerializeField] private Slider m_MasterSlider;
    [SerializeField] private Slider m_BGMSlider;
    [SerializeField] private Slider m_BGSSlider;
    [SerializeField] private Slider m_SFXSlider;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_SFXPage;
    
    private bool m_isPaused;
    private int m_CurrentSection;

    private float m_PlayerMinPosX = -7.5f;
    private float m_PlayerMinPosY = -70.0f;
    private float m_PlayerMaxPosX = 70.0f;
    private float m_PlayerMaxPosY = 7.5f;

    private float m_IconMinPosX = -39.0f;
    private float m_IconMinPosY = -358f;
    private float m_IconMaxPosX = 359f;
    private float m_IconMaxPosY = 39.0f;

    void Start()
    {
        PauseGame(false);
    }

    void Update()
    {
        if(GameManager.instance.getSceneID() == 13) return;
        if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)))
        {
            if(GameManager.instance.getSceneID() == 0 && m_isPaused == false) return;
            PauseGame(!m_isPaused);
        }
    }

    public bool isPaused()
    {
        return m_isPaused;
    }

    public void PauseGame(bool state)
    {
        if(state == false)
        {
            m_CanvasGroup.alpha = 0.0f;
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;
            Time.timeScale = 1.0f;
        }
        else 
        {
            m_CanvasGroup.alpha = 1.0f;
            m_CanvasGroup.interactable = true;
            m_CanvasGroup.blocksRaycasts = true;
            m_CurrentSection = -1;

            int titleID = 0;
            m_MapButton.SetActive(GameManager.instance.getSceneID() != titleID);
            m_ItemButton.SetActive(GameManager.instance.getSceneID() != titleID);
            m_SystemButton.SetActive(true);

            if(GameManager.instance.getSceneID() == titleID)
            {
                OpenSystemScreen();
            }
            else 
            {
                OpenMapScreen();
            }

            Time.timeScale = 0.0f;
        }
        m_isPaused = state;
    }

    public void OpenMapScreen()
    {
        if(m_CurrentSection == 0) return;
        m_CurrentSection = 0;
        SetCurrentPage();
        PlayFlipSound();
        FlipPage();
        
        Vector2 mapPos = Vector2.zero;
        if(GameManager.instance.isOutside())
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            mapPos = player.transform.position;

            float convertX = Mathf.Abs((m_IconMinPosX + m_IconMaxPosX) / (m_PlayerMinPosX + m_PlayerMaxPosX));
            float convertY = Mathf.Abs((m_IconMinPosY + m_IconMaxPosY) / (m_PlayerMinPosY + m_PlayerMaxPosY));
            float multi = 1.0f;
            float newX = Mathf.Clamp(player.transform.position.x * (convertX * multi), m_IconMinPosX, m_IconMaxPosX);
            float newY = Mathf.Clamp(player.transform.position.y * (convertY * multi), m_IconMinPosY, m_IconMaxPosY);

            mapPos = new Vector2(newX, newY);
        }
        else 
        {
            switch(GameManager.instance.getSceneID())
            {
                case 2:
                    // Player
                    mapPos = new Vector2(261,-230);
                    break;
                case 3:
                    // School
                    mapPos = new Vector2(192,10);
                    break;
                case 4:
                    // Cafe
                    mapPos = new Vector2(4.5f,-70);
                    break;
                case 5:
                    // House Helena
                    mapPos = new Vector2(109,-155);
                    break;
                case 6:
                    // House Mycen
                    mapPos = new Vector2(72,-217);
                    break;
                case 7:
                    // House Jan
                    mapPos = new Vector2(128,-75);
                    break;
                case 8:
                    // House Barn
                    mapPos = new Vector2(334,-158);
                    break;
                case 9:
                    // House Couple
                    mapPos = new Vector2(199,-75);
                    break;
                case 10:
                    // House Fisher
                    mapPos = new Vector2(227,-157);
                    break;
                case 11:
                    // House Grannie
                    mapPos = new Vector2(283,-30);
                    break;
                case 12:
                    // Barn
                    mapPos = new Vector2(288.5f,-190);
                    break;
            }
        }
        m_IconTransform.anchoredPosition = mapPos;
    }

    public void OpenItemsScreen()
    {
        if(m_CurrentSection == 1) return;
        m_CurrentSection = 1;
        ClearItemDescription();
        SetCurrentPage();
        PlayFlipSound();
        FlipPage();

        int c = m_InventoryHolder.childCount;
        for(int i = 0; i < c; i++)
        {
            GameObject child = m_InventoryHolder.transform.GetChild(i).gameObject;
            
            if(i < GameManager.instance.m_Items.Count)
            {
                child.SetActive(true);
                ItemSlot itemSlot = child.GetComponentInChildren<ItemSlot>();
                Item item = GameManager.instance.m_Items[i];
                itemSlot.SetItemSlot(item.itemName, item.itemDescription, item.sprite, 0);
            }
            else if(i == GameManager.instance.m_Items.Count && GameManager.instance.m_FishAmount>0)
            {
                child.SetActive(true);
                ItemSlot itemSlot = child.GetComponentInChildren<ItemSlot>();
                itemSlot.SetItemSlot(m_FishItem.itemName, m_FishItem.itemDescription, m_FishItem.sprite, GameManager.instance.m_FishAmount);
            }
            else
            {
                child.SetActive(false);
            }
        }

        m_QuestLog.text = "";
        foreach(Quest quest in GameManager.instance.m_AllQuests)
        {
            if(GameManager.instance.m_CompletedQuests.Contains(quest))
            {
                m_QuestLog.text += "<s>" + quest.description + "</s>";
            }
            else 
            {
                m_QuestLog.text += quest.description;
            }
            m_QuestLog.text += "\n";
        }
    }

    public void OpenSystemScreen()
    {
        if(m_CurrentSection == 2) return;
        m_CurrentSection = 2;
        SetCurrentPage();
        PlayFlipSound();
        FlipPage();

        m_MasterSlider.value = GameManager.instance.GetMasterVolume();
        m_BGMSlider.value = GameManager.instance.GetBGMVolume();
        m_BGSSlider.value = GameManager.instance.GetBGSVolume();
        m_SFXSlider.value = GameManager.instance.GetSFXVolume();
    }

    void SetCurrentPage()
    {
        m_MapHolder.SetActive(m_CurrentSection == 0);
        m_ItemHolder.SetActive(m_CurrentSection == 1);
        m_SystemHolder.SetActive(m_CurrentSection == 2);
    }

    void FlipPage()
    {
        StopCoroutine(FlippingPage());
        StartCoroutine(FlippingPage());
    }

    void PlayFlipSound()
    {
        m_AudioSource.clip = m_SFXPage;
        m_AudioSource.pitch = 1.0f;
        m_AudioSource.Play();
    }

    public void SetItemDescription(string name, string description)
    {
        m_ItemName.text = name;
        m_ItemDescription.text = description;
    }

    public void ClearItemDescription()
    {
        SetItemDescription("","");
    }

    public void AdjustMasterVolume(float value)
    {
        GameManager.instance.SetMasterVolume(value);
    }

    public void AdjustBGMVolume(float value)
    {
        GameManager.instance.SetBGMVolume(value);
    }

    public void AdjustBGSVolume(float value)
    {
        GameManager.instance.SetBGSVolume(value); 
    }

    public void AdjustSFXVolume(float value)
    {
        GameManager.instance.SetSFXVolume(value);
    }

    public void MapleEasterEgg()
    {
        StartCoroutine(NGIO.UnlockMedal(72300));
        Application.OpenURL("https://www.youtube.com/watch?v=6rmgyLd9A0k");
    }

    public void VisitProfile(string name)
    {
        if(m_CurrentSection != 2 && m_isPaused == false) return;

        Application.OpenURL("https://" + name + ".newgrounds.com/");
    }

    IEnumerator FlippingPage()
    {
        Vector2 startSize = Vector2.one * 1.01f;
        Vector2 endSize = Vector2.one;
        float t = 0.0f;
        float flipDur = 0.2f;
        while(t < 1.0f)
        {
            t += Time.unscaledDeltaTime / flipDur;
            m_CanvasTransform.localScale = Vector2.Lerp(startSize, endSize, t);
            yield return null;
        }
        m_CanvasTransform.localScale = endSize;
    }
}
