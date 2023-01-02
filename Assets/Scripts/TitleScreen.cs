using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour
{
    [Header("Components")]
    public GameObject m_PressStartText;
    public GameObject m_NGIO;
    
    [Header("Main Menu")]
    public GameObject MainMenuWrapper;
    public Button NewGameButton;
    public Button OptionsButton;

    private bool m_Started;
    private bool m_Paused;

    // Start is called before the first frame update
    void Start()
    {
        NewGameButton.onClick.AddListener(this.StartGame);
        OptionsButton.onClick.AddListener(this.OnOptionsButtonClicked);
        m_PressStartText.SetActive(true);
    }

    void Update()
    {
        if(m_Started) return;
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Space))
        {
            m_Started = true;
            m_NGIO.SetActive(true);
        }

        if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && m_Paused)
        {
            ShowMainMenu();
            m_Paused = false;
        }
    }

    // this will be called once the API has finished loading everything
    public void OnNewgroundsIOReady(BaseEventData e)
    {
        StartCoroutine(NGIO.UnlockMedal(72296));
        ShowMainMenu();
    }

    public void SetMenuVisibility(bool mainMenu=true, bool continueMenu=true)
    {
        MainMenuWrapper.SetActive(mainMenu);
    }

    public void HideMenus()
    {
        SetMenuVisibility(false,false);
    }

    public void ShowMainMenu() 
    {
        SetMenuVisibility(true,false);
        m_PressStartText.SetActive(false);
    }

    public void ShowOptionsMenu()
    {
        FindObjectOfType<PauseManager>().PauseGame(true);
    }

    public void StartGame() 
    {
        GameManager.instance.TransferPlayer(2, Vector2.zero, true);
    }

    public void OnOptionsButtonClicked() {
        ShowOptionsMenu();
        HideMenus();
        m_Paused = true;
    }
}
