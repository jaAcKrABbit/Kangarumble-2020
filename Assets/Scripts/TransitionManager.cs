using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    private GameObject mainCam;
    private GameObject countdownObj;

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

    private bool gameEndCheck;
    private bool startedCountdown;

    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        countdownObj = GameObject.Find("321Rumble");

        transitionMask.SetActive(true);
        transitionOverlay.SetActive(true);

        maskScale = (open) ? maskScaleOpen : maskScaleClose;
        transitionMask.transform.localScale = new Vector3(maskScale, maskScale, 1);
        maskYPos = (open) ? maskYPosOpen: maskYPosClose;
        transitionMask.transform.position = new Vector3(maskXPos, maskYPos, 0);

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

        if (open && fullScreenOpen && !startedCountdown) {
            startedCountdown = true;
            countdownObj.GetComponent<CountdownToFight>().StartCountdown();
        }
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

        if (gameEndCheck && fullScreenClose) {
            SceneManager.LoadScene("EndScene");
        }
    }

    void StartOpen()
    {
        open = true;
    }

    void GameEndClose()
    {
        open = false;
        gameEndCheck = true;
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
