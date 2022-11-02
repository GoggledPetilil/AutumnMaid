using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageSpr : MonoBehaviour
{
    [SerializeField] private float m_ActiveTime = 0.2f;
    private float m_TimeActivated;
    private float m_Alpha;
    [SerializeField] private float m_AlphaSet = 0.8f;
    [SerializeField] private float m_AlphaMultiplier = 0.85f;

    private Transform m_Obj;

    private SpriteRenderer m_sr;
    private SpriteRenderer m_ObjSr;

    [SerializeField] private Color m_Color;
    private Color m_CurrentColor;

    void OnEnable()
    {
        m_sr = GetComponent<SpriteRenderer>();
        m_Obj = GameObject.FindGameObjectWithTag("Broom").transform.GetChild(0);
        m_ObjSr = m_Obj.GetComponentInChildren<SpriteRenderer>();

        m_Alpha = m_AlphaSet;
        m_sr.sprite = m_ObjSr.sprite;
        transform.position = m_Obj.position;
        transform.rotation = m_Obj.rotation;

        m_TimeActivated = Time.time;
    }

    void SetColor()
    {
        m_CurrentColor = new Color(m_Color.r, m_Color.g, m_Color.b, m_Alpha);
        m_sr.color = m_CurrentColor;
    }

    void Update()
    {
        m_Alpha *= m_AlphaMultiplier;
        SetColor();

        if(Time.time >= (m_TimeActivated + m_ActiveTime))
        {
            AfterImagePool.instance.AddToPool(gameObject);
        }
    }
}
