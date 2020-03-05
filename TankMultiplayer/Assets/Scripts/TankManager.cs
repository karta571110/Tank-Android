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

    public void GetColorAndPoint(int i)
    {
        switch (i)
        {
            case 0:
                m_PlayerColor = new Color(72f / 255f, 123f / 255f, 207f / 255f, 255f / 255f);
                m_SpawnPoint = GameObject.Find("SpawnPoint" + (i + 1)).transform;
                break;
            case 1:
                m_PlayerColor = new Color(203f / 255f, 31f / 255f, 31f / 255f);
                m_SpawnPoint = GameObject.Find("SpawnPoint" + (i + 1)).transform;
                break;
            case 2:
                m_PlayerColor = new Color(197f / 255f, 96f / 255f, 181f / 255f);
                m_SpawnPoint = GameObject.Find("SpawnPoint" + (i + 1)).transform;
                break;
            case 3:
                m_PlayerColor = new Color(36f / 255f, 183f / 255f, 37f / 255f);
                m_SpawnPoint = GameObject.Find("SpawnPoint" + (i + 1)).transform;
                break;
        }
    }

    public void Setup()
    {

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
