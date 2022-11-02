using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Map Management")]
    [SerializeField] private GameObject[] m_WorldPieces;
    [SerializeField] private float m_XOffset;
    [SerializeField] private float m_YOffset;
    public bool m_Generate;

    [Header("Render Settings")]
    [SerializeField] private float m_RenderDistance;
    private GameObject m_OriginPoint;

    void Awake()
    {
        m_OriginPoint = Camera.main.gameObject;
    }

    void Start()
    {
        LayWorldGrids();
        DisableAllTiles();
    }

    void Update()
    {
        if(m_Generate)
        {
            LayWorldGrids();
            m_Generate = false;
            return;
        }

        RenderTiles();
    }

    void LayWorldGrids()
    {
        int maxX = 4;
        int maxY = 4;
        int currentX = 0;
        int currentY = 0;
        for(int i = 0; i < ((maxX+1) * (maxY+1)); i++)
        {
            float newX = 0.0f + (m_XOffset * currentX);
            float newY = 0.0f - (m_YOffset * currentY);
            Vector3 newPos = new Vector3(newX, newY, 0.0f);

            m_WorldPieces[i].transform.position = newPos;

            if(currentY >= maxY)
            {
                currentY = 0;
                if(currentX >= maxX)
                {
                    currentX = 0;
                }
                else 
                {
                    currentX++;
                }
            }
            else 
            {
                currentY++;
            }
        }
    }

    void DisableAllTiles()
    {
        for(int i = 0; i < m_WorldPieces.Length; i++)
        {
            m_WorldPieces[i].SetActive(false);
        }
    }

    void RenderTiles()
    {
        foreach(GameObject tile in m_WorldPieces)
        {
            float distanceSqr = (m_OriginPoint.transform.position - tile.transform.position).sqrMagnitude;
            if(distanceSqr < m_RenderDistance)
            {
                tile.SetActive(true);
            }
            else 
            {
                tile.SetActive(false);
            }
        }
    }
}
