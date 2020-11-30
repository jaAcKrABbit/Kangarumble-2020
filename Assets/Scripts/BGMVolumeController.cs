using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMVolumeController : MonoBehaviour{
    
    private AudioSource audioSrc;
    private float audioVol = 0.2f;
    // Start is called before the first frame update
    void Start(){
        audioSrc = GetComponent<AudioSource>();   
    }

    // Update is called once per frame
    void Update(){
        audioSrc.volume = audioVol;
    }

    public void SetVolume(float vol) {
        audioVol = vol;
    }
}
