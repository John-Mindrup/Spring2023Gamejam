using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }
    private AudioSource audioSource;
    [SerializeField] private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(Instance == null)
        {

            Instance = this;
        }
        else
        {
            audioSource.volume = Instance.audioSource.volume;
        }
    }

    public static void setVolume()
    {
        Instance.audioSource.volume = Instance.slider.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
