using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup m_Canvas;
    [SerializeField] private Animator m_LureAnim;
    [SerializeField] private Animator m_FishCursor;
    [SerializeField] private Animator m_FishPortrait;
    [SerializeField] private Animator m_MaplePortrait;
    [SerializeField] private Slider m_FishingSlider;
    [SerializeField] private Animator m_SliderAnim;
    [SerializeField] private Dialogue m_WinDialogue;
    
    [Header("Fishing Components")]
    [SerializeField] private Transform m_SeaPosition;   // The position of the ocean.
    [SerializeField] private Transform m_FishingPosition;   // The position where the Player will be fishing.
    [SerializeField] private GameObject m_FishingLure;
    [SerializeField] private ParticleSystem m_SplashParticles;
    private bool m_IsFishing;
    private float m_LureLowest = -10.0f;    // Lure Position when FishHP is max.
    private float m_LureHighest = -5.4f;    // Lure Position when FishHP is 0.
    private float m_LurePosition;   // What position the Lure is between its lowest and highest.
    private float m_FishNoticeTime = 1.2f;   // The amount of time the player has to notice the fish.
    private int m_FishingStage;
    private float m_WaitTime;   // How long before you hook in a fish.
    private bool m_SliderGoRight; 
    private float m_FishingCoolDown;    // Time until you can fish/cancel again.
    private bool m_NoticedFish;

    [Header("Fish Parameters")]
    [SerializeField] private Fish[] m_CommonFish;
    [SerializeField] private Fish[] m_UncommonFish;
    [SerializeField] private Fish[] m_RareFish;
    [SerializeField] private Fish m_HookedFish;
    [SerializeField] private GameObject m_FishObject;
    private float m_FishMaxHP;
    private float m_FishHP;
    private bool m_CalmFish;
    private float m_FishRage;       // How long rage lasts
    private float m_NextRage;       // How long till next Rage

    [Header("Audio Components")]
    [SerializeField] private AudioSource m_aud;
    [SerializeField] private AudioSource m_FishingPoleAS;
    [SerializeField] private AudioSource m_LureAS;
    [SerializeField] private AudioClip m_RageSFX;
    [SerializeField] private AudioClip m_ReelSFX;
    [SerializeField] private AudioClip m_CastSFX;
    [SerializeField] private AudioClip m_SplashSFX;
    [SerializeField] private AudioClip m_HookSFX;
    [SerializeField] private AudioClip m_FallingSFX;
    [SerializeField] private AudioClip m_ItemGetSFX;

    [Header("Fisherman Components")]
    [SerializeField] private SpriteRenderer m_FisherRenderer;
    [SerializeField] private Sprite m_FisherIdleSprite;
    [SerializeField] private Sprite m_FisherShockSprite;

    private Player m_Player;

    void Awake()
    {
        m_FishingPoleAS.clip = m_ReelSFX;
        m_FishingPoleAS.loop = true;
        m_FishingLure.SetActive(false);
    }

    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        if(m_FishingStage == 1)
        {
            if(m_WaitTime > 0.0f)
            {
                m_WaitTime -= Time.deltaTime;

                if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.K))
                {
                    if(m_FishingCoolDown <= 0.0f)
                    {
                        // Reeled in too early.
                        StopFishing();
                    }
                }
            }
            else 
            {
                m_FishingStage = 2;
            }
        }
        else if(m_FishingStage == 2)
        {
            if(m_NoticedFish == false)
            {
                m_Player.m_ani.SetTrigger("noticedFish");
                m_LureAnim.SetInteger("animState", 2);
                m_LureAS.clip = m_HookSFX;
                m_LureAS.Play();
                PlaySplash();

                m_NoticedFish = true;
            }

            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.K))
            {
                if(m_WaitTime <= (0.0f - m_FishNoticeTime))
                {
                    FailFishing();
                }
                else 
                {
                    GetFishInfo();
                    m_FishingStage = 3;
                }

            }
            else if(m_WaitTime > (0.0f - m_FishNoticeTime))
            {  
                m_WaitTime -= Time.deltaTime;
            }
        }
        else if(m_FishingStage == 3)
        {
            // The fish will now have to be caught.
            // If calm, use Z or K to deplete its health
            // If agitated, wait for it to calm.

            if(m_FishHP > 0.0f)
            {
                // Adjust the slider.
                float sliderMod = (m_CalmFish) ? 1.0f : 1.8f;
                if(m_SliderGoRight)
                {
                    if(m_FishingSlider.value >= m_FishingSlider.maxValue)
                    {
                        m_SliderGoRight = false;
                    }
                    else
                    {
                        m_FishingSlider.value += Time.deltaTime * sliderMod;
                    }
                }
                else 
                {
                    if(m_FishingSlider.value <= m_FishingSlider.minValue)
                    {
                        m_SliderGoRight = true;
                    }
                    else
                    {
                        m_FishingSlider.value -= Time.deltaTime * sliderMod;
                    }
                }

                // Fish Rage Controller
                if(m_CalmFish)
                {
                    if(m_NextRage > 0.0f)
                    {
                        m_NextRage -= Time.deltaTime;
                    }
                    else 
                    {
                        m_FishRage = m_HookedFish.rageDuration;
                        m_CalmFish = false;
                        m_FishPortrait.SetBool("isRaging", true);
                        PlayAudio(m_RageSFX);
                    }
                }
                else 
                {
                    if(m_FishRage > 0.0f)
                    {
                        m_FishRage -= Time.deltaTime;
                    }
                    else 
                    {
                        m_NextRage = m_HookedFish.rageFrequency;
                        m_CalmFish = true;
                        m_FishPortrait.SetBool("isRaging", false);
                    }
                }

                // Player controller
                if(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.K))
                {
                    m_Player.m_ani.SetBool("reelingIn", true);
                    m_FishCursor.SetBool("isDown", true);
                    if(!m_FishingPoleAS.isPlaying) m_FishingPoleAS.Play();

                    float val = m_FishingSlider.value;
                    if((val > 0.0f && val < 0.1f) || (val > 0.9f && val < 1.0f))
                    {
                        // Red
                        m_FishHP += Time.deltaTime * 5.0f;
                    }
                    else if((val > 0.1f && val < 0.235f) || (val > 0.76f && val < 1.0f))
                    {
                        // Orange
                        m_FishHP += Time.deltaTime * 2.5f;
                    }
                    else if((val > 0.235f && val < 0.375f) || (val > 0.625f && val < 0.76f))
                    {
                        // Yellow
                        m_FishHP += Time.deltaTime * 0.5f;
                    }
                    else if((val > 0.375f && val < 0.468f) || (val > 0.53f && val < 0.625f))
                    {
                        // Green
                        m_FishHP -= Time.deltaTime * 2.0f;
                    }
                    else if(val > 0.468f && val < 0.53f)
                    {
                        // Blue
                        m_FishHP -= Time.deltaTime * 4.5f;
                    }
                }
                else 
                {
                    m_Player.m_ani.SetBool("reelingIn", false);
                    m_FishCursor.SetBool("isDown", false);
                    m_FishingPoleAS.Stop();

                    m_FishHP += Time.deltaTime * 0.25f;
                }

                float fishHealth = m_FishHP / m_FishMaxHP;
                m_LurePosition = 1.0f - fishHealth;
                float newY = Mathf.Lerp(m_LureLowest, m_LureHighest, m_LurePosition);

                m_FishingLure.transform.localPosition = new Vector2(m_FishingLure.transform.localPosition.x, newY);
            }
            else if(m_FishHP <= 0.0f)
            {
                CatchFish();
            }
            
            if(m_FishHP >= (m_FishMaxHP + 1.0f))
            {
                // Too much stress!!
                FailFishing();
            }
        }
        else if(m_FishingStage == 4)
        {
            // Will end fishing if not in a textbox.
            if(GameManager.instance.isTalking() == false)
            {
                StopFishing();
            }
        }

        if(m_FishingCoolDown > 0.0f)
        {
            m_FishingCoolDown -= Time.deltaTime;
        }
    }

    public void StartFishing()
    {
        if(m_IsFishing || m_FishingCoolDown > 0.0f || GameManager.instance.m_IsDelivering || 
        GameManager.instance.m_FlagMetFisher == false) return;
        StartCoroutine("StartFishingEvent");
    }

    void GetFishInfo()
    {
        float r = Random.Range(0, 512);
        if(r == 0)
        {
            m_HookedFish = m_RareFish[Random.Range(0, m_RareFish.Length)];
        }
        else if(r <= 16) 
        {
            m_HookedFish = m_UncommonFish[Random.Range(0, m_UncommonFish.Length)];
        }
        else 
        {
            m_HookedFish = m_CommonFish[Random.Range(0, m_CommonFish.Length)];
        }
        
        m_FishMaxHP = m_HookedFish.fishHealth;
        m_FishHP = m_FishMaxHP;
        m_FishRage = m_HookedFish.rageDuration;
        m_NextRage = m_HookedFish.rageFrequency;

        m_CalmFish = true;
        m_FishPortrait.SetBool("isRaging", false);

        // Open UI ELements
        ToggleUIElements(true);
    }

    void ToggleUIElements(bool state)
    {
        if(state == true) m_Canvas.alpha = 1.0f;

        m_MaplePortrait.SetBool("isOpen", state);
        m_FishPortrait.SetBool("isOpen", state);
        m_SliderAnim.SetBool("isOpen", state);
    }

    void PlayAudio(AudioClip clip)
    {
        m_aud.clip = clip;
        m_aud.Play();
    }
    
    void CatchFish()
    {
        m_FishingStage = 0;
        StartCoroutine(CaughtFish());
    }

    void FailFishing()
    {
        Debug.Log("No fish...");

        StopFishing();
    }

    void StopFishing()
    {
        ToggleUIElements(false);
        GameManager.instance.SetCamFollower(m_SeaPosition.gameObject);

        m_FisherRenderer.sprite = m_FisherIdleSprite;
        
        m_IsFishing = false;
        m_FishingPoleAS.Stop();
        m_FishingLure.SetActive(false);
        m_FishObject.SetActive(false);

        m_Player.StopMovement(false);
        m_Player.m_ani.SetBool("isFishing", false);
        m_Player.m_ani.SetBool("reelingIn", false);
        m_Player.m_ani.SetInteger("fishWin", 0);

        m_WaitTime = 0.0f;
        m_FishingStage = 0;
        m_SliderGoRight = true;
        
        FishCoolDown();
    }

    void PlaySplash()
    {
        m_SplashParticles.transform.position = m_FishingLure.transform.position;
        m_SplashParticles.Play();
    }

    void FishCoolDown()
    {
        m_FishingCoolDown = 0.1f;
    }

    IEnumerator StartFishingEvent()
    {
        // Move Player towards pier
        float t = 0.0f;
        Vector2 playPos = m_Player.transform.position;
        Vector2 newPos = m_FishingPosition.position;
        float movDur = 0.2f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / movDur;
            m_Player.transform.position = Vector2.Lerp(playPos, newPos, t);
            m_Player.m_ani.SetBool("isMoving", true);
            yield return null;
        }
        m_Player.m_ani.SetBool("isMoving", false);
        
        // Set up fishing parameters
        m_IsFishing = true;
        m_NoticedFish = false;
        m_SliderGoRight = true;
        m_FishObject.SetActive(false);
        m_Player.StopMovement(true);
        m_Player.m_ani.SetBool("isFishing", true);
        m_FishingSlider.value = 0.5f;
        m_LurePosition = 0.0f;

        // Animation delay
        yield return new WaitForSeconds(0.2f);
        
        // Cast Lure
        m_LureAS.clip = m_CastSFX;
        m_LureAS.Play();
        m_FishingLure.SetActive(true);
        m_LureAnim.SetInteger("animState", 0);
        t = 0.0f;
        Vector2 lureStart = new Vector3(5.0f, -3.2f, 0.0f);
        Vector2 lureEnd = new Vector3(m_FishingLure.transform.localPosition.x, m_LureLowest, m_FishingLure.transform.localPosition.z);
        float lurDur = 0.6f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / lurDur;
            m_FishingLure.transform.localPosition = Vector2.Lerp(lureStart, lureEnd, t);
            
            float scaleMod = 2.5f;
            Vector2 startScale = Vector2.one;
            Vector2 endScale = Vector2.one *scaleMod;
            if(t > 0.5f)
            {
                startScale = Vector2.one *scaleMod;
                endScale = Vector2.one;
            }
            m_FishingLure.transform.localScale = Vector2.Lerp(startScale, endScale, t);
            yield return null;
        }
        PlaySplash();
        m_LureAS.clip = m_SplashSFX;
        m_LureAS.Play();
        m_LureAnim.SetInteger("animState", 1);

        // Start fishing
        FishCoolDown();
        m_WaitTime = Random.Range(5.0f, 10.0f);
        m_FishingStage = 1;

        yield return null;
    }

    IEnumerator CaughtFish()
    {
        ToggleUIElements(false);
        m_FishingPoleAS.Stop();
        GameManager.instance.SetCamFollower(m_Player.gameObject);

        m_FishingLure.SetActive(false);
        m_FishObject.SetActive(true);
        m_FishObject.GetComponent<SpriteRenderer>().sprite = m_HookedFish.sprite;

        // Have fish go from lure all the way up to the screen
        PlaySplash();
        m_LureAS.clip = m_SplashSFX;
        m_LureAS.Play();
        m_Player.m_ani.SetInteger("fishWin", 1);
        m_FisherRenderer.sprite = m_FisherShockSprite;
        Vector2 startFlyPos = m_FishingLure.transform.localPosition;
        Vector2 endFlyPos = new Vector2(startFlyPos.x, startFlyPos.y + 20f);
        float flyDur = 1.25f;
        float t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / flyDur;
            m_FishObject.transform.localPosition = Vector2.Lerp(startFlyPos, endFlyPos, t);
            yield return null;
        }
        // Then back down again into the players hand
        if(m_Player.isFacingRight() == false) m_Player.FlipSprite(false);
        m_Player.m_ani.SetInteger("fishWin", 2);
        Vector2 startFallPos = endFlyPos;
        Vector2 endFallPos = new Vector2(4.0f, -2.8f);
        float fallDur = 1.5f;
        t = 0.0f;
        PlayAudio(m_FallingSFX);
        while(t < 1.0f)
        {
            t += Time.deltaTime / fallDur;
            m_FishObject.transform.localPosition = Vector2.Lerp(startFallPos, endFallPos, t);
            yield return null;
        }
        m_Player.m_ani.SetInteger("fishWin", 3);
        PlayAudio(m_ItemGetSFX);

        // Textbox: You got some dogshit!
        string msg = "Wow! I caught a " + m_HookedFish.itemName + "!";
        if(m_HookedFish.isNotFish)
        {
            m_Player.m_ani.SetInteger("fishWin", 4);
            msg = "Wha- This isn't a fish at all! I've been bamboozled!";
        }
        else 
        {
            GameManager.instance.m_FishAmount++;
        }
        m_WinDialogue.dialogueInfo[1].sentence = msg;

        FindObjectOfType<DialogueManager>().StartDialogue(m_WinDialogue);
        yield return new WaitForSeconds(0.2f);
        m_FishingStage = 4;

        yield return null;
    }
}
