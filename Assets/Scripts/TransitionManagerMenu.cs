using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManagerMenu : MonoBehaviour
{
    public GameObject transitionOverlay;
    public GameObject transitionMask;

    public bool open;
    public bool fullScreenOpen;
    public bool fullScreenClose;

    private float maskScale;
    public float maskScaleOpen;
    public float maskScaleClose;
    public float maskScaleSpeedOpen;
    public float maskScaleSpeedClose;

    public float maskXPos;
    private float maskYPos;
    public float maskYPosOpenRatio;
    public float maskYPosCloseRatio;
    private float maskYPosOpen;
    private float maskYPosClose;

    private bool menuEndCheck;
    private bool startedCountdown;
    public bool creditsCheck;
    public bool creditsCheckOver;
    public bool endSceneCheck;

    void Start()
    {
        transitionMask.SetActive(true);
        transitionOverlay.SetActive(true);

        maskScale = (open) ? maskScaleOpen : maskScaleClose;
        transitionMask.transform.localScale = new Vector3(maskScale, maskScale, 1);
        maskYPos = (open) ? maskYPosOpen: maskYPosClose;
        transitionMask.transform.position = new Vector3(maskXPos, maskYPos, 0);

        endSceneCheck = false;
        creditsCheck = false;
        creditsCheckOver = false;
        startedCountdown = false;
        open = false;
        Invoke("StartOpen", 0.5f);
    }

    void Update()
    {
        transitionMask.transform.localScale = new Vector3(maskScale, maskScale, 1);
        maskXPos = Screen.width / 2;

        fullScreenOpen = Mathf.Abs(maskScale - maskScaleOpen) < 3f;
        fullScreenClose = Mathf.Abs(maskScale - maskScaleClose) < 0.01f;
    }

    void FixedUpdate()
    {
        float maskScaleDest = (open) ? maskScaleOpen : maskScaleClose;
        float maskScaleSpeed = (open) ? maskScaleSpeedOpen : maskScaleSpeedClose;
        maskScale = Damp(maskScale, maskScaleDest, maskScaleSpeed, Time.deltaTime);

        maskYPosOpen = Screen.height * maskYPosOpenRatio;
        maskYPosClose = Screen.height * maskYPosCloseRatio;
        float maskYPosDest = (open) ? maskYPosOpen: maskYPosClose;
        maskYPos = Damp(maskYPos, maskYPosDest, maskScaleSpeed, Time.deltaTime);
        transitionMask.transform.position = new Vector3(maskXPos, maskYPos, 0);

        if (menuEndCheck && fullScreenClose) {
            SceneManager.LoadScene("Loading");
        }

        if (creditsCheck && fullScreenClose) {
            SceneManager.LoadScene("Credits");
        }

        if (creditsCheckOver && fullScreenClose) {
            SceneManager.LoadScene("OpeningMenu");
        }

        if (endSceneCheck && fullScreenClose) {
            SceneManager.LoadScene("OpeningMenu");
        }
    }

    void StartOpen()
    {
        open = true;
    }

    public void MenuEndClose()
    {
        open = false;
        menuEndCheck = true;
    }

    public void CreditsClose()
    {
        open = false;
        creditsCheck = true;
    }

    public void CreditsOver()
    {
        open = false;
        creditsCheckOver = true;
    }

    public void EndSceneClose()
    {
        open = false;
        endSceneCheck = true;
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
