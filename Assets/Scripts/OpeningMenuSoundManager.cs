using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningMenuSoundManager : MonoBehaviour
{
    public AudioClip[] charSelectSFX;

    [SerializeField]
    private AudioSource audioSrc;

    public void playCharSelectSound(int charID)
    {
        if(charSelectSFX[charID] != null)
            audioSrc.PlayOneShot(charSelectSFX[charID]);
    }
}
