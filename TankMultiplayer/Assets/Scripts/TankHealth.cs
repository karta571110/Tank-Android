using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public AudioClip m_TankExplosionClip;

    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        //找物件
        for(int i = 0; i < transform.childCount; i++)
        {
            switch (transform.GetChild(i).name)
            {
                case "TankExplosion":
                    m_ExplosionParticles = transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    break;
                case "Canvas":
                    for(int j=0;j< transform.GetChild(i).childCount; j++)
                    {
                        switch (transform.GetChild(i).gameObject.transform.GetChild(j).name)
                        {
                            case "HealthSlider":
                                m_Slider = transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.GetComponent<Slider>();
                                for(int k = 0; k < m_Slider.gameObject.transform.childCount; k++)
                                {
                                    switch (m_Slider.gameObject.transform.GetChild(k).name)
                                    {
                                        case "Fill Area":
                                            for(int l=0;l< m_Slider.gameObject.transform.GetChild(k).gameObject.transform.childCount; l++)
                                            {
                                                switch (m_Slider.gameObject.transform.GetChild(k).gameObject.transform.GetChild(l).name)
                                                {
                                                    case "Fill":
                                                        m_FillImage = m_Slider.gameObject.transform.GetChild(k).gameObject.transform.GetChild(l).gameObject.GetComponent<Image>();
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        m_ExplosionAudio = GetComponent<AudioSource>();

        m_TankExplosionClip = Resources.Load<AudioClip>("AudioClips/TankExplosion");

        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_ExplosionParticles.gameObject.SetActive(false);
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount;

        SetHealthUI();

        if (m_CurrentHealth <= 0 && !m_Dead)
            OnDeath();       
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.

        m_Slider.value = m_CurrentHealth;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.

        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();

        m_ExplosionAudio.PlayOneShot(m_TankExplosionClip);


        gameObject.SetActive(false);
    }
}