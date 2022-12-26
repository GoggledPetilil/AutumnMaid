using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_IconTransform;
    
    private bool m_isPaused;

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
            Time.timeScale = 1.0f;
            m_CanvasGroup.alpha = 0.0f;

            m_isPaused = false;
        }
        else 
        {
            Time.timeScale = 0.0f;
            m_CanvasGroup.alpha = 1.0f;
            OpenMapScreen();

            m_isPaused = true;
        }
    }

    void OpenMapScreen()
    {
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
                case 0:
                    mapPos = Vector2.zero;
                    break;
            }
        }
        m_IconTransform.anchoredPosition = mapPos;
    }
}
