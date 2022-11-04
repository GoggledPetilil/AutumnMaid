using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup m_Canvas;
    [SerializeField] private Animator m_FishCursor;
    [SerializeField] private Animator m_FishPortrait;
    [SerializeField] private Animator m_MaplePortrait;
    [SerializeField] private Animator m_SliderAnim;
    [SerializeField] private Animator m_HealthAnim;
    [SerializeField] private Slider m_FishingSlider;
    [SerializeField] private Slider m_HPSlider;
    
    [Header("Fishing Components")]
    [SerializeField] private bool m_IsFishing;
    [SerializeField] private int m_FishingStage;
    [SerializeField] private float m_WaitTime;   // How long before you hook in a fish.
    [SerializeField] private bool m_SliderGoRight; 
    private float m_FishingCoolDown;    // Time until you can fish again.

    [Header("Fish Parameters")]
    [SerializeField] private float m_FishMaxHP;
    [SerializeField] private float m_FishHP;
    [SerializeField] private bool m_CalmFish;
    [SerializeField] private float m_MaxRage;
    [SerializeField] private float m_CurrentRage;

    [Header("Audio Components")]
    [SerializeField] private AudioSource m_aud;
    [SerializeField] private AudioSource m_FishingPoleAS;
    [SerializeField] private AudioClip m_RageSFX;
    [SerializeField] private AudioClip m_ReelSFX;

    private Player m_Player;

    void Awake()
    {
        m_FishingPoleAS.clip = m_ReelSFX;
        m_FishingPoleAS.loop = true;
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
                    // Reeled in too early.
                    StopFishing();
                }
            }
            else 
            {
                m_FishingStage = 2;
            }
        }
        else if(m_FishingStage == 2)
        {
            Debug.Log("Got a fish!! Hook it in!");
            m_Player.m_ani.SetTrigger("noticedFish");
            // Player notices fish.
            // Press Z or K to hook it in!
            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.K))
            {
                GetFishInfo();
                m_FishingStage = 3;
            }
            else if(m_WaitTime > -3.2f)
            {  
                m_WaitTime -= Time.deltaTime;
            }
            else 
            {
                FailFishing();
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
                float sliderMod = (m_CalmFish) ? 1.2f : 2.0f;
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
                    if(m_CurrentRage < m_MaxRage)
                    {
                        m_CurrentRage += Time.deltaTime;
                    }
                    else 
                    {
                        m_CalmFish = false;
                        m_FishPortrait.SetBool("isRaging", true);
                        PlayAudio(m_RageSFX);
                    }
                }
                else 
                {
                    if(m_CurrentRage > 0.0f)
                    {
                        m_CurrentRage -= Time.deltaTime;
                    }
                    else 
                    {
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
                        m_FishHP -= Time.deltaTime * 1.5f;
                    }
                    else if(val > 0.468f && val < 0.53f)
                    {
                        // Blue
                        m_FishHP -= Time.deltaTime * 4.2f;
                    }
                }
                else 
                {
                    m_Player.m_ani.SetBool("reelingIn", false);
                    m_FishCursor.SetBool("isDown", false);
                    m_FishingPoleAS.Stop();

                    m_FishHP += Time.deltaTime * 0.25f;
                }

                SetHealthSlider();
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

        if(m_FishingCoolDown > 0.0f)
        {
            m_FishingCoolDown -= Time.deltaTime;
        }
    }

    public void StartFishing()
    {
        if(m_IsFishing || m_FishingCoolDown > 0.0f) return;
        m_IsFishing = true;

        m_Player.StopMovement(true);
        m_Player.m_ani.SetBool("isFishing", true);

        m_SliderGoRight = true;
        m_FishingSlider.value = 0.5f;

        m_WaitTime = Random.Range(5.0f, 10.0f);
        m_FishingStage = 1;
    }

    void GetFishInfo()
    {
        m_FishMaxHP = 4f;
        m_FishHP = m_FishMaxHP;
        SetHealthSlider();
        m_MaxRage = 10f;
        m_CurrentRage = 0.0f;

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
        m_HealthAnim.SetBool("isOpen", state);
    }

    void SetHealthSlider()
    {
        m_HPSlider.value = m_FishHP / m_FishMaxHP;
    }

    void PlayAudio(AudioClip clip)
    {
        m_aud.clip = clip;
        m_aud.Play();
    }
    
    void CatchFish()
    {
        m_FishingStage = 4;
        Debug.Log("Caught a fish!");
        
        StopFishing();
    }

    void FailFishing()
    {
        Debug.Log("No fish...");

        StopFishing();
    }

    void StopFishing()
    {
        ToggleUIElements(false);
        
        m_IsFishing = false;
        m_FishingPoleAS.Stop();

        m_Player.StopMovement(false);
        m_Player.m_ani.SetBool("isFishing", false);
        m_Player.m_ani.SetBool("reelingIn", false);

        ToggleUIElements(false);

        m_WaitTime = 0.0f;
        m_FishingStage = 0;
        m_SliderGoRight = true;
        
        m_FishingCoolDown = 0.1f;
    }
}
