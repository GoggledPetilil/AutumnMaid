using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Parameters")]
    public float m_Speed;
    public float m_SpeedMod;

    [Header("Move Physics")]
    public Vector2 m_MovDir;
    public Vector2 m_LookDir;
    public GameObject m_TargetPoint;
    public float m_TargetDistance;
    private bool m_DisableControls;
    private Vector2 m_TargetOffset = new Vector2(0.0f, 1.0f);

    [Header("Broom Parameters")]
    public GameObject m_AttackPoint;
    public float m_DistanceMod;

    [Header("Components")]
    public Animator m_ani;
    [SerializeField] private Collider2D m_col;
    [SerializeField] private GameObject m_PromptHolder;
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private SpriteRenderer m_sr;
    [SerializeField] private PlayerBroom m_broom;
    public Interactable m_InteractEvent;

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
    }

    void Update()
    {
        Move();

        if(m_InteractEvent != null)
        {
            if(m_DisableControls) return;

            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.K))
            {
                PromptAppear(false);
                m_InteractEvent.InvokeEvent();

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

        m_ani.SetBool("isMoving", m_rb.velocity.magnitude > 0);
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
    }

    public void DisableColliders(bool state)
    {
        m_col.isTrigger = state;
    }

    public void PromptAppear(bool state)
    {
        m_PromptHolder.SetActive(state);
    }
}
