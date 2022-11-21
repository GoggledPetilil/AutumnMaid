using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector2 m_NewPosition;
    public List<int> m_SheepOnHand = new List<int>();
    public List<int> m_SheepSaved = new List<int>();
    public int m_FishAmount;

    [Header("Components")]
    [SerializeField] private Animator m_Anim;
    [SerializeField] private AudioSource m_BGMPlayer;
    [SerializeField] private AudioSource m_SFXPlayer;

    [Header("System Audio")]
    [SerializeField] private AudioClip m_TransferSFX;

    [Header("Effects Prefabs")]
    [SerializeField] private GameObject m_SparklesPrefabs;

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
        if(song == m_BGMPlayer.clip) return;
        
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
        Debug.Log("Yippee! You did something good!");
    }

    public void LoadNewScene(int sceneIndex)
    {
        StartCoroutine(TransferSequence(sceneIndex, Vector2.zero, false));
    }

    public void TransferPlayer(int sceneIndex, Vector2 newPos, bool zoomTransfer)
    {
        StartCoroutine(TransferSequence(sceneIndex, newPos, zoomTransfer));
    }

    IEnumerator TransferSequence(int sceneIndex, Vector2 newPos, bool zoomTransfer)
    {
        if(zoomTransfer)
        {
            var camera = Camera.main;
            var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
            var vcam = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;

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
        
        m_NewPosition = newPos;

        if(!zoomTransfer) PlaySFX(m_TransferSFX);
        m_Anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
        m_Anim.SetTrigger("FadeIn");
    }
}
