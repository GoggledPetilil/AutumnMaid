using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector2 m_NewPosition;
    public List<int> m_SheepOnHand = new List<int>();
    public List<int> m_SheepSaved = new List<int>();
    public int m_FishAmount;
    public bool m_IsDelivering;

    [Header("Components")]
    [SerializeField] private Animator m_Anim;
    [SerializeField] private AudioSource m_BGMPlayer;
    [SerializeField] private AudioSource m_SFXPlayer;

    [Header("Music Parameters")]
    private float m_MusicTime = 64;     // Time between each song.
    private float m_NextSong;           // Time until next song.

    [Header("System Audio")]
    [SerializeField] private AudioClip m_TransferSFX;

    [Header("Effects Prefabs")]
    [SerializeField] private GameObject m_SparklesPrefabs;

    [Header("Level Up Components")]
    [SerializeField] private CanvasGroup m_GroupHolder;
    [SerializeField] private CanvasGroup m_LevelUpTint;
    [SerializeField] private CanvasGroup m_HeartHolder;
    [SerializeField] private Image m_HeartFill;
    [SerializeField] private Image m_HeartGlow;
    [SerializeField] private RectTransform m_GlowTransform;
    [SerializeField] private AudioClip m_FadeSFX;
    [SerializeField] private AudioClip m_IncreaseSFX;
    public int m_HappyLevel;
    private int m_MaxHappiness = 10;

    [Header("Flags")]
    public bool m_FlagCleanedMapleRoom;
    public bool m_FlagMetFarmer;
    public bool m_FlagSavedSheep;
    public bool m_FlagMetFisher;
    public bool m_FlagThankFisher;
    public bool m_FlagFedCat;
    public bool m_FlagMetOldLady;
    public bool m_FlagHaveGlasses;
    public bool m_FlagLadyHelped;
    public bool m_FlagMetCafe;
    public int m_DeliveryStage;
    public bool m_FlagMetGhost;
    public bool m_FlagHaveMemento;
    public bool m_FlagGhostThanks;
    public bool m_FlagHaveTea;
    public bool m_FlagDrankTea;
    public bool m_FlagMetPostman;
    public bool m_FlagHaveToolbox;
    public bool m_FlagThanksPostman;
    public bool m_FlagAskedDIY;
    public bool m_FlagMetHelena;
    public bool m_FlagCleanedHelenaRoom;
    public bool m_FlagHelenaBook;
    public bool m_FlagCheckedMycen;
    public bool m_FlagMycenDoorOpen;
    public bool m_FlagMetOswald;
    public bool m_FlagMetHilda;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            m_Anim = GetComponentInChildren<Animator>();

            DontDestroyOnLoad(this.gameObject);
        }
        else 
        {
            Destroy(this.gameObject);
        }
    }

    public void SetCamFollower(GameObject followObj)
    {
        var camera = Camera.main;
        var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
        var vcam = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        vcam.m_Follow = followObj.transform;
    }

    public void PlayBGM(AudioClip song)
    {
        if((m_BGMPlayer.isPlaying && m_BGMPlayer.loop == false)) return;

        if(isOutside())
        {
            if(Time.time > m_NextSong)
            {
                // When cooldown has passed, add new cooldown.
                m_NextSong = Time.time + song.length + m_MusicTime;
            }
            else 
            {
                // If it's still under cooldown, then don't play anything.
                m_BGMPlayer.Stop();
                return;
            }

            if(m_BGMPlayer.loop == true)
            {
                // Music doesn't loop when outside.
                m_BGMPlayer.loop = false;
            }
        }
        else
        {
            if(m_BGMPlayer.isPlaying)
            {
                // Indoor music will not cutout outdoor music.
                song = m_BGMPlayer.clip;
            }
            else 
            {
                // Music loops when indoors.
                m_BGMPlayer.loop = true;

                // Only play the indoor music sometimes.
                int r = Random.Range(0, 4);
                if(r != 0)
                {
                    m_BGMPlayer.Stop();
                    return;
                }
            }
        }
        
        m_BGMPlayer.clip = song;
        m_BGMPlayer.Play();
    }

    public void PlaySFX(AudioClip sound)
    {
        m_SFXPlayer.clip = sound;
        m_SFXPlayer.Play();
    }

    public void SpawnSparkles(Vector3 pos)
    {
        GameObject go = Instantiate(m_SparklesPrefabs, pos, Quaternion.identity) as GameObject;
    }

    public void GetItem(Item item)
    {
        InventoryManager inv = GetComponentInChildren<InventoryManager>();
        inv.GetItem(item);
    }

    public void AddSheep(int sheepID)
    {
        if(m_SheepOnHand.Contains(sheepID)) return;
        m_SheepOnHand.Add(sheepID);
    }

    public void RemoveSheep(int sheepID)
    {
        if(!m_SheepOnHand.Contains(sheepID)) return;
        m_SheepOnHand.Remove(sheepID);
    }

    public void SaveSheep(int sheepID)
    {
        if(m_SheepSaved.Contains(sheepID)) return;
        m_SheepSaved.Add(sheepID);
    }

    public void IncreaseHappiness()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.StopMovement(true);
        StartCoroutine(LevelUp());
    }

    public void LoadNewScene(int sceneIndex)
    {
        StartCoroutine(TransferSequence(sceneIndex, Vector2.zero, false));
    }

    public void TransferPlayer(int sceneIndex, Vector2 newPos, bool zoomTransfer)
    {
        StartCoroutine(TransferSequence(sceneIndex, newPos, zoomTransfer));
    }

    public void StartDelivery(bool state)
    {
        GameManager.instance.m_IsDelivering = state;
        
        Player p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if(state == true)
        {
            p.CarryPizza(m_DeliveryStage);
        }
        else 
        {
            p.CarryPizza(-1);
        }
    }

    public void HeartFill()
    {
        m_HeartFill.fillAmount = Mathf.Clamp((float)m_HappyLevel / (float)m_MaxHappiness, 0.0f, 1.0f);
    }

    public bool isTalking()
    {
        return FindObjectOfType<DialogueManager>().isTalking();
    }

    public bool isOutside()
    {
        bool isOutside = false;
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            isOutside = true;
        }
        return isOutside;
    }

    public int getSceneID()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    IEnumerator TransferSequence(int sceneIndex, Vector2 newPos, bool zoomTransfer)
    {
        if(zoomTransfer)
        {
            var camera = Camera.main;
            var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
            var vcam = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;

            if(vcam != null)
            {
                float farSize = vcam.m_Lens.OrthographicSize;
                float zoomSize = vcam.m_Lens.OrthographicSize * 0.75f;
                float zoomDur = 0.75f;
                float t = 0.0f;
                while(t < 1.0f)
                {
                    t += Time.deltaTime / zoomDur;
                    vcam.m_Lens.OrthographicSize = Mathf.Lerp(farSize, zoomSize, t);
                    yield return null;
                }
            }
        }
        
        m_NewPosition = newPos;

        if(!zoomTransfer) PlaySFX(m_TransferSFX);
        m_Anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
        m_Anim.SetTrigger("FadeIn");
    }

    IEnumerator LevelUp()
    {
        // Initialize Everything
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_GroupHolder.alpha = 1.0f;
        m_HeartHolder.alpha = 0.0f;
        Color heartColor = new Color(0.859f, 0.267f, 0.49f);
        m_HeartFill.color = heartColor;
        m_HeartGlow.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        HeartFill();
        
        // Tint screen black
        float blackDur = 1.0f;
        float t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / blackDur;
            m_LevelUpTint.alpha = t;
            yield return null;
        }

        // Grow Heart on screen (with current fill)
        float heartDur = 1.0f;
        t = 0.0f;
        PlaySFX(m_FadeSFX);
        while(t < 1.0f)
        {
            t += Time.deltaTime / heartDur;
            m_HeartHolder.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.32f);

        // Make fill bigger + white to pink fade
        // Glow effect
        m_HappyLevel++;
        float fillDur = 1.0f;
        t = 0.0f;
        Color startColor = Color.white;
        Color endColor = heartColor;
        Color glowStart = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        Color glowEnd = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        Vector2 startSize = new Vector2(1.1f, 1.1f);
        Vector2 endSize = new Vector2(1.6f, 1.6f);
        PlaySFX(m_IncreaseSFX);
        HeartFill();
        while(t < 1.0f)
        {
            t += Time.deltaTime / fillDur;
            m_HeartFill.color = Color.Lerp(startColor, endColor, t);
            m_GlowTransform.localScale = Vector2.Lerp(startSize, endSize, t);
            m_HeartGlow.color = Color.Lerp(glowStart, glowEnd, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        player.PlayLevelUp();

        // Make heart shrink again
        t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / heartDur;
            m_HeartHolder.alpha = 1.0f - t;
            yield return null;
        }

        // Get rid of tint
        t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / blackDur;
            m_LevelUpTint.alpha = 1.0f - t;
            yield return null;
        }

        player.StopMovement(false);
    }
}
