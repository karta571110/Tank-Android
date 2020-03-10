using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class TankShooting : MonoBehaviourPunCallbacks
{
    public int m_PlayerNumber = 1;
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f;
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f;

    GameObject shell;
    GameObject shellTemp;

    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;

    int i = 0;//使射擊順暢
    private void Awake()
    {
        //預置砲彈
        for (int i = 0; i < transform.childCount; i++)
        {
            switch (transform.GetChild(i).name)
            {
                case "ShellTemp":
                    shellTemp = transform.GetChild(i).gameObject;
                    shellTemp.name += m_PlayerNumber;
                    
                    break;
                case "Canvas":
                    for (int j = 0; j < transform.GetChild(i).transform.childCount; j++)
                    {
                        switch (transform.GetChild(i).transform.GetChild(j).name)
                        {
                            case "AimSlider":
                                m_AimSlider = transform.GetChild(i).transform.GetChild(j).GetComponent<Slider>();
                                break;
                        }
                    }
                    break;
                case "FireTransform":
                    m_FireTransform = transform.GetChild(i).transform;
                    m_ShootingAudio = m_FireTransform.GetComponent<AudioSource>();
                    break;
            }
        }
        shellTemp.transform.SetParent(null);

        m_ChargingClip = Resources.Load<AudioClip>("AudioClips/ShotCharging");
        m_FireClip = Resources.Load<AudioClip>("AudioClips/ShotFiring");

        shell = Resources.Load<GameObject>("Prefabs/Shell");

        
        for (int i = 0; i < 50; i++)
        {
            Instantiate(shell, new Vector3(0, 0, -300), Quaternion.identity).transform.SetParent(shellTemp.transform);
        }
    }
    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;//距離=時間*速度
    }


    private void Update()
    {
        m_AimSlider.value = m_MinLaunchForce;
        // Track the current state of the fire button and make decisions based on the current launch force.
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.

        m_Fired = true;

        if (i > 30)
            i = 0;

        Rigidbody shellRigibody = shellTemp.transform.GetChild(i).GetComponent<Rigidbody>();
        shellRigibody.gameObject.SetActive(true);
        shellRigibody.gameObject.transform.position = m_FireTransform.position;
        shellRigibody.gameObject.transform.rotation = m_FireTransform.rotation;
        shellRigibody.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.PlayOneShot(m_FireClip);

        m_CurrentLaunchForce = m_MinLaunchForce;

        i++;
    }
}