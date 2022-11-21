using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : NPC
{
    [Header("Parameters")]
    public int m_ID;
    private float m_Speed = 4.85f;
    private float m_FollowDistance = 2.5f;    // Distance threshold between the Sheep and Player.
    private float m_StopDistance = 8.2f;      // Distance threshold to stop following the Player
    private Transform m_FollowTarget;         // Entity that's being followed.
    private Vector2 m_MovDir;

    [Header("Components")]
    [SerializeField] private Animator m_Anim;
    [SerializeField] private Rigidbody2D m_rb;

    void Start()
    {
        if(GameManager.instance.m_SheepSaved.Contains(m_ID))
        {
            this.gameObject.SetActive(false);
        }
        else 
        {
            GameManager.instance.RemoveSheep(m_ID);
        }
    }

    void Update()
    {
        if(m_FollowTarget == null) return;

        if(Vector2.Distance(m_FollowTarget.position, transform.position) > m_FollowDistance)
        {
            if(Vector2.Distance(m_FollowTarget.position, transform.position) < m_StopDistance)
            {
                Follow();
            }
            else 
            {
                GameManager.instance.RemoveSheep(m_ID);
                m_FollowTarget = null;
                m_MovDir = Vector2.zero;
            }
        }
        else
        {
            m_MovDir = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        m_rb.velocity = m_MovDir * m_Speed;
        m_Anim.SetBool("isMoving", m_rb.velocity.magnitude > 0.01f);
        if (m_rb.velocity != Vector2.zero)
        {
            if(m_MovDir.x > 0.0f)
            {
                FlipSprite(false);
            }
            else if(m_MovDir.x < 0.0f)
            {
                FlipSprite(true);
            }
        }
    }

    void Follow()
    {
        m_MovDir = (m_FollowTarget.position - transform.position).normalized;
    }

    public void FollowPlayer()
    {
        Player p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if(m_FollowTarget == null && GameManager.instance.m_FlagMetFarmer)
        {
            transform.parent = null;    // Sheep will unparent itself from the acre, so it doesn't despawn when the acre does.
            
            m_FollowTarget = p.gameObject.transform;
            GameManager.instance.AddSheep(m_ID);

            m_FollowDistance = m_FollowDistance * Random.Range(0.9f, 1.05f);
        }
        else 
        {
            p.PatObject(this);
        }
    }
}
