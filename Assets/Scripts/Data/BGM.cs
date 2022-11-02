using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] private AudioClip m_BGM;

    void Start()
    {
        GameManager.instance.PlayBGM(m_BGM);
        this.gameObject.SetActive(false);
    }
}
