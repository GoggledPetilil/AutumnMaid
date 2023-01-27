using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public enum TerrainTag
    {
        Dirt,
        Stone,
        Wood,
        Sand
    }
    
    [Header("Parameters")]
    public float m_Speed;
    public float m_SpeedMod;
    public TerrainTag m_CurrentTerrain;
    public LayerMask m_CollisionLayers;
    private float m_PatDistance = 1.5f;

    [Header("Move Physics")]
    public Vector2 m_MovDir;
    public Vector2 m_LookDir;
    public GameObject m_TargetPoint;
    public float m_TargetDistance;
    private bool m_DisableControls;
    private bool m_DisableEvents;
    private Vector2 m_TargetOffset = new Vector2(0.0f, 1.0f);

    [Header("Broom Parameters")]
    public GameObject m_AttackPoint;
    public float m_DistanceMod;

    [Header("Components")]
    public Animator m_ani;
    [SerializeField] private ParticleSystem m_LevelParticles;
    [SerializeField] private Collider2D m_col;
    [SerializeField] private GameObject m_PromptHolder;
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private SpriteRenderer m_sr;
    [SerializeField] private PlayerBroom m_broom;
    public Interactable m_InteractEvent;

    [Header("Audio Components")]
    [SerializeField] private AudioSource m_StepAudio;
    [SerializeField] private AudioClip[] m_SFXStepDirt;
    [SerializeField] private AudioClip[] m_SFXStepStone;
    [SerializeField] private AudioClip[] m_SFXStepSand;
    [SerializeField] private AudioClip[] m_SFXStepWood;

    [Header("Item Components")]
    [SerializeField] private SpriteRenderer m_PizzaRenderer;
    [SerializeField] private Sprite[] m_PizzaSprites;
    

    void Awake()
    {
        m_broom = GetComponentInChildren<PlayerBroom>();
    }

    void Start()
    {
        transform.position = GameManager.instance.m_NewPosition;
        
        m_LookDir = Vector2.right;
        SetTargetPoint();

        m_SpeedMod = 1;

        PromptAppear(false);
        if(GameManager.instance.m_IsDelivering)
        {
            CarryPizza(GameManager.instance.m_DeliveryStage);
        }

        if(GameManager.instance.m_BigMode)
        {
            m_sr.transform.localScale = new Vector2(2.0f, 1.0f);
        }
        else 
        {
            m_sr.transform.localScale = Vector2.one;
        }
    }

    void Update()
    {
        Move();

        if(m_InteractEvent != null)
        {
            if(m_DisableEvents) return;

            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.K))
            {
                m_broom.StopAttack();
                PromptAppear(false);
                m_InteractEvent.InvokeEvent();

                if(m_InteractEvent.isActiveAndEnabled)
                {
                    if(transform.position.x > m_InteractEvent.transform.position.x)
                    {
                        FlipSprite(true);
                        m_LookDir = Vector2.left;
                    }
                    else if(transform.position.x < m_InteractEvent.transform.position.x)
                    {
                        FlipSprite(false);
                        m_LookDir = Vector2.right;
                    }
                }

                SetTargetPoint();
            }
        }
    }

    void FixedUpdate()
    {
        m_rb.velocity = m_MovDir * GetSpeed();
    }

    void Move()
    {
        if(!m_DisableControls)
        {
            m_MovDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }
        
        if(m_MovDir != Vector2.zero)
        {
            m_LookDir = m_MovDir;
            SetTargetPoint();
        }

        m_ani.SetBool("isMoving", m_rb.velocity.magnitude > 0.1f);
        if(m_MovDir.x < 0)
        {
            FlipSprite(true);
        }
        else if(m_MovDir.x > 0)
        {
            FlipSprite(false);
        }
    }

    private void SetTargetPoint()
    {
        m_TargetPoint.transform.localPosition = (m_LookDir + m_TargetOffset) * m_TargetDistance;
        m_AttackPoint.transform.localPosition = ((m_LookDir * m_DistanceMod) + m_TargetOffset) * m_TargetDistance;
    }

    private float GetSpeed()
    {
        float speed = (5 + (0.5f * m_Speed)) * m_SpeedMod;
        return speed;
    }

    public void StopMovement(bool state)
    {
        if(state == true)
        {
            m_MovDir = Vector2.zero;
            m_rb.velocity = Vector2.zero;
            m_ani.SetBool("isMoving", false);
        }
        m_broom.enabled = !state;
        m_DisableControls = state;
        m_DisableEvents = state;
    }

    public bool canMove()
    {
        return !m_DisableControls;
    }

    public void StopMovement(bool state, bool disableEvents)
    {
        if(state == true)
        {
            m_MovDir = Vector2.zero;
            m_rb.velocity = Vector2.zero;
            m_ani.SetBool("isMoving", false);
        }
        m_broom.enabled = !state;
        m_DisableControls = state;
        m_DisableEvents = disableEvents;
    }

    public void DisableColliders(bool state)
    {
        m_col.isTrigger = state;
    }

    public void PromptAppear(bool state)
    {
        m_PromptHolder.SetActive(state);
    }

    public void AppearAbove(bool state)
    {
        if(state == true)
        {
            m_sr.sortingOrder = 99;
        }
        else 
        {
            m_sr.sortingOrder = 0;
        }
    }

    public void PlayFootstep()
    {
        if(!GameManager.instance.isOutside() || m_CurrentTerrain == TerrainTag.Wood)
        {
            m_StepAudio.clip = m_SFXStepWood[Random.Range(0, m_SFXStepWood.Length)];
        }  
        else if(m_CurrentTerrain == TerrainTag.Dirt)
        {
            m_StepAudio.clip = m_SFXStepDirt[Random.Range(0, m_SFXStepDirt.Length)];
        }
        else if(m_CurrentTerrain == TerrainTag.Stone)
        {
            m_StepAudio.clip = m_SFXStepStone[Random.Range(0, m_SFXStepStone.Length)];
        }
        else if(m_CurrentTerrain == TerrainTag.Sand)
        {
            m_StepAudio.clip = m_SFXStepSand[Random.Range(0, m_SFXStepSand.Length)];
        }
        m_StepAudio.Play();
    }

    public void PatObject(Entity obj)
    {
        if(GameManager.instance.m_IsDelivering) return;

        // Check if no colliders are in the way.
        float distance = (this.transform.position.x > obj.transform.position.x) ? m_PatDistance : -m_PatDistance;
        RaycastHit2D hit = Physics2D.Raycast(obj.transform.position, Vector2.right, distance, m_CollisionLayers);

        if(hit.collider == null)
        {
            StartCoroutine(PatThis(obj));
        }
    }

    public void CarryPizza(int amount)
    {
        if(amount < 0)
        {
            m_ani.SetBool("isHolding", false);
        }
        else 
        {
            m_ani.SetBool("isHolding", true);
            m_PizzaRenderer.sprite = m_PizzaSprites[Mathf.Clamp(amount, 0, m_PizzaSprites.Length)];
        }
    }

    public void PlayLevelUp()
    {
        m_LevelParticles.Play();
    }

    IEnumerator PatThis(Entity obj)
    {
        StopMovement(true);

        Vector2 startPos = this.transform.position;
        Vector2 endPos = new Vector2(obj.transform.position.x, obj.transform.position.y - 0.01f);
        float distance = (startPos.x > endPos.x) ? m_PatDistance : -m_PatDistance;
        endPos.x += distance;
        float t = 0.0f;
        float walkDur = Vector2.Distance(transform.position, endPos) / 3.2f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / walkDur;
            this.gameObject.transform.position = Vector2.Lerp(startPos, endPos, t);
            m_ani.SetBool("isMoving", true);
            yield return null;
        }
        m_ani.SetBool("isMoving", false);

        t = 0.0f;
        float patDur = 1.0f;
        Animator oAnim = obj.GetComponent<Animator>();
        while(t < 1.0f)
        {
            t += Time.deltaTime / patDur;
            m_ani.SetBool("isPatting", true);
            oAnim.SetBool("isPat", true);
            yield return null;
        }
        m_ani.SetBool("isPatting", false);
        oAnim.SetBool("isPat", false);

        StopMovement(false);
    }
}
