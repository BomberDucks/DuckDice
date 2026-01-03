using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    
    public static BackgroundMusic Instance;

    [Header("Réglages")]
    public AudioClip MusicClip;
    [Range(0f, 1f)] public float Volume = 0.5f;

    private AudioSource audioSource;

    private void Awake()
    {
        //singleton pour une seule musiqui qui reste
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        
        Instance = this;
        DontDestroyOnLoad(gameObject); 

        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = MusicClip;
        audioSource.loop = true; 
        audioSource.playOnAwake = true;
        audioSource.volume = Volume;
        audioSource.Play();
    }

    private void Update()
    {
        
        if (audioSource != null)
        {
            audioSource.volume = Volume;
        }
    }
}