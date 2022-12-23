using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachBall : MonoBehaviour
{
    [Header("Parameters")]
    public float m_Speed;
    [SerializeField] private float m_ShadowOffset;
    [SerializeField] private Vector2 m_MovDir;

    [Header("Components")]
    [SerializeField] private AudioSource m_Audiosource;
    [SerializeField] private AudioSource m_SplashAudio;
    [SerializeField] private Collider2D m_BallCollider;
    [SerializeField] private Collider2D m_EntityCollider;
    [SerializeField] private Transform m_Shadow;
    [SerializeField] private ParticleSystem m_WaterSplash;
    [SerializeField] private Rigidbody2D m_rb;

    void Update()
    {
        float xVelocity = m_rb.velocity.magnitude * Mathf.Sign(m_MovDir.x);
        m_rb.transform.eulerAngles = new Vector3(0.0f, 0.0f, m_rb.transform.eulerAngles.z - xVelocity);
        m_Shadow.position = new Vector2(m_rb.transform.position.x, m_rb.transform.position.y + m_ShadowOffset);
    }

    public void BounceBall(Transform origin, bool isKicked)
    {
        m_Audiosource.Play();
        m_MovDir = (transform.position - origin.position).normalized;
        if(isKicked)
        {
            m_rb.AddForce(m_MovDir * m_Speed, ForceMode2D.Impulse);
        }
        else 
        {
            m_rb.AddForce(m_MovDir * (m_rb.velocity.magnitude / 1.5f), ForceMode2D.Impulse);
        }
    }

    void DrownBall()
    {
        m_WaterSplash.transform.position = new Vector2(m_rb.transform.position.x, m_rb.transform.position.y + m_ShadowOffset);
        m_rb.gameObject.SetActive(false);

        m_WaterSplash.Play();
        m_SplashAudio.Play();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Water"))
        {
            Vector3 other = col.collider.ClosestPoint(m_rb.transform.position);
            m_BallCollider.enabled = false;
            m_EntityCollider.enabled = false;
            m_Shadow.gameObject.SetActive(false);

            // Shoot into current direction
            m_MovDir = (other - m_rb.transform.position).normalized;
            m_rb.AddForce(m_MovDir * 4.0f, ForceMode2D.Impulse);

            Invoke("DrownBall", 0.25f);
        }
        else
        {
            // Bounce into oppsite direction.
            BounceBall(col.transform, col.gameObject.CompareTag("Player"));
        }
    }
}
