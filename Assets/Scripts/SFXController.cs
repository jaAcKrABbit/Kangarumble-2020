using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour {

    private AudioSource oneShotSrc;
    private AudioSource conveyorbeltSrc;
    private float audioVol = 0.2f;
    // Start is called before the first frame update
    void Start() {
        oneShotSrc = GetComponent<SoundManager>().audioSrc;
        conveyorbeltSrc = GetComponent<SoundManager>().conveyorBeltSound;
    }

    // Update is called once per frame
    void Update() {
        oneShotSrc.volume = audioVol;
        conveyorbeltSrc.volume = audioVol;
    }

    public void SetVolume(float vol) {
        audioVol = vol;
    }
}