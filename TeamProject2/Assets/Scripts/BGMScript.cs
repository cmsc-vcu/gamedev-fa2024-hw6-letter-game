using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMScript : MonoBehaviour
{
    public AudioClip introMusic;
    public AudioClip loopMusic;

    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!source.isPlaying)
        {
            source.clip = loopMusic;
            source.loop = true;
            source.Play();
        }
    }
}
