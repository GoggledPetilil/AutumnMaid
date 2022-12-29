using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_CanvasTransform;
    [SerializeField] private GameObject m_MapHolder;
    [SerializeField] private RectTransform m_IconTransform;
    [SerializeField] private GameObject m_ItemHolder;
    [SerializeField] private GameObject m_SystemHolder;
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!m_isPaused);
        }
    }

    void PauseGame(bool state)
    {
        if(state == false)
        {
            m_CanvasGroup.alpha = 0.0f;
            Time.timeScale = 1.0f;
        }
        else 
        {
            if(GameManager.instance.getSceneID() != 0)
            {
                m_CanvasGroup.alpha = 1.0f;
                m_CurrentSection = -1;
                OpenMapScreen();
                Time.timeScale = 0.0f;
            }
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
        SetCurrentPage();
        PlayFlipSound();
        FlipPage();
    }

    public void OpenSystemScreen()
    {
        if(m_CurrentSection == 2) return;
        m_CurrentSection = 2;
        SetCurrentPage();
        PlayFlipSound();
        FlipPage();
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

    public void AdjustMasterVolume(float value)
    {
        GameManager.instance.AdjustMasterVolume(value);
    }

    public void AdjustBGMVolume(float value)
    {
        GameManager.instance.AdjustBGMVolume(value);
    }

    public void AdjustBGSVolume(float value)
    {
        GameManager.instance.AdjustBGSVolume(value); 
    }

    public void AdjustSFXVolume(float value)
    {
        GameManager.instance.AdjustSFXVolume(value);
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
