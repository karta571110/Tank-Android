using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;                     


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        switch (m_PlayerNumber)
        {
            case 1:
                m_PlayerColor = new Color(72f, 123f, 207f);
                m_SpawnPoint = GameObject.Find("SpawnPoint" + m_PlayerNumber).transform;
                break;
            case 2:
                m_PlayerColor = new Color(203f, 31f, 31f);
                m_SpawnPoint = GameObject.Find("SpawnPoint" + m_PlayerNumber).transform;
                break;
            case 3:
                m_PlayerColor = new Color(197f, 96f, 181f);
                m_SpawnPoint = GameObject.Find("SpawnPoint" + m_PlayerNumber).transform;
                break;
            case 4:
                m_PlayerColor = new Color(36f, 183f, 37f);
                m_SpawnPoint = GameObject.Find("SpawnPoint" + m_PlayerNumber).transform;
                break;
        }
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
