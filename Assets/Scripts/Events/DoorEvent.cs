using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DoorEvent : MonoBehaviour
{
    [Header("Warp Info")]
    public int m_MapID;
    public Vector2 m_WarpCords;
    public bool m_ZoomTransfer;

    [Header("Components")]
    [SerializeField] private AudioSource m_aud;
    [SerializeField] private AudioClip m_DoorSFX;
    [SerializeField] private GameObject m_DoorSprite;
    
    public void Awake()
    {
        if(m_DoorSprite != null)
        {
            m_DoorSprite.SetActive(false);
        }
    }

    public void TriggerDoor()
    {
        StartCoroutine(DoorSequence());
    }

    IEnumerator DoorSequence()
    {
        // Stop Player Movement
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.StopMovement(true);
        player.DisableColliders(true);
        if(m_DoorSprite != null)
        {
            SortingGroup sg = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SortingGroup>();
            sg.sortingOrder = 99;
        }

        // Open door sprite
        if(m_DoorSprite != null) m_DoorSprite.SetActive(true);
        m_aud.clip = m_DoorSFX;
        m_aud.pitch = Random.Range(0.9f, 1.0f);
        m_aud.Play();

        yield return new WaitForSeconds(0.1f);

        // Move Player towards door
        float t = 0.0f;
        Vector2 playPos = player.transform.position;
        Vector2 doorPos = this.transform.position;
        float movDur = Vector2.Distance(playPos, doorPos) / 5.0f;
        GameManager.instance.SetCamFollower(player.gameObject);
        while(t < 1.0f)
        {
            t += Time.deltaTime / movDur;
            player.transform.position = Vector2.Lerp(playPos, doorPos, t);
            player.m_ani.SetBool("isMoving", true);
            yield return null;
        }
        player.m_ani.SetBool("isMoving", false);
        // Change scene!
        GameManager.instance.TransferPlayer(m_MapID, m_WarpCords, m_ZoomTransfer);
    }
}
