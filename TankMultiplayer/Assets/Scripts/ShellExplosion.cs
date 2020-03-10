using UnityEngine;
using System.Collections;
using Photon.Pun;
public class ShellExplosion : MonoBehaviourPunCallbacks
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;
    public AudioClip m_ExplosionAudioClip;
    public float m_MaxDamage = 100f;
    public float m_ExplosionForce = 1000f;
    public float m_MaxLifeTime = 2f;
    public float m_ExplosionRadius = 5f;

    MeshRenderer m_meshRenderer;
    CapsuleCollider m_capsuleCollider;
    Light m_light;

    public GameObject shellTemp;
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            switch (transform.GetChild(i).name)
            {
                case "ShellExplosion":
                    m_ExplosionParticles = transform.GetChild(i).GetComponent<ParticleSystem>();
                    break;
            }
        }
        m_light = GetComponentInChildren<Light>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_capsuleCollider = GetComponent<CapsuleCollider>();
        shellTemp = transform.parent.gameObject;
        m_ExplosionAudio = GetComponentInChildren<AudioSource>();
        m_ExplosionAudioClip = Resources.Load<AudioClip>("AudioClips/ShellExplosion");
    }
    private void OnEnable()
    {
        gameObject.SetActive(true);
        m_light.enabled = true;
        m_meshRenderer.enabled = true;
        m_capsuleCollider.enabled = true;
    }
    private void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (!photonView.IsMine)
        {
            return;
        }
        

        // Find all the tanks in an area around the shell and damage them.

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigibody = colliders[i].GetComponent<Rigidbody>();//抓取爆炸半徑內的Rigidbody

            if (!targetRigibody)
                continue;

            targetRigibody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigibody.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigibody.position);

            targetHealth.TakeDamage(damage);
        }

        StartCoroutine(Explsion());

    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;//求單一座標點或兩座標點之間的距離長度

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }

    IEnumerator Explsion()
    {
        m_meshRenderer.enabled = false;
        m_capsuleCollider.enabled = false;
        m_light.enabled = false;
        transform.SetParent(null);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.PlayOneShot(m_ExplosionAudioClip);

        yield return new WaitForSeconds(m_MaxLifeTime);

        gameObject.SetActive(false);
        transform.SetParent(shellTemp.transform);
    }
}
