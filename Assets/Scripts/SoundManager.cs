using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] jumpSound;
    public AudioClip wallJumpSound;
    public AudioClip[] stompSound = new AudioClip[4];
    public AudioClip quickStompSound;

    public AudioClip turnSwitchSound;

    public bool onBelt = false;

    [SerializeField]
    public AudioSource audioSrc;
    [SerializeField]
    public AudioSource conveyorBeltSound;
    public AudioClip pipeSound;

    public void PlayJumpSound()
    {
        int jumpSoundIndex = Random.Range(0, jumpSound.Length);
        audioSrc.PlayOneShot(jumpSound[jumpSoundIndex]);
    }
    public void PlayWallJumpSound()
    {
        audioSrc.PlayOneShot(wallJumpSound);
    }

    public void PlayStompSound(int charIndex)
    {
        audioSrc.PlayOneShot(stompSound[charIndex]);
    }

    public void PlayQuickStompSound()
    {
        audioSrc.PlayOneShot(quickStompSound);
    }

    public void PlayTurnSwitchSound()
    {
        audioSrc.PlayOneShot(turnSwitchSound);
    }
    public void PlayPipeSound()
    {
        audioSrc.PlayOneShot(pipeSound);
    }

    void Update()
    {
        if(onBelt && !conveyorBeltSound.isPlaying)
        {
            //Debug.Log("YES");
            conveyorBeltSound.Play();
        }
        if(!onBelt && conveyorBeltSound.isPlaying)
        {
            //Debug.Log("NO");
            conveyorBeltSound.Stop();
        }
    }
}
