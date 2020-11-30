using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstMenu : MonoBehaviour
{
    public bool inputTimer;
    public float inputDelayTime;
    public bool canContinue;

    private GameObject openingMenuObj;
    private GameObject levelSettingsObj;
    private GameObject transitionManager;

    private GameObject playObj;
    private GameObject creditsObj;
    private GameObject quitObj;
    private GameObject arrowLeftObj;
    private GameObject arrowRightObj;
    private GameObject[] menuItems = new GameObject[3];

    private bool hoveringOnPlay;
    private bool hoveringOnCredits;
    private bool hoveringOnQuit;

    private float arrowLeftX = 0;
    private float arrowRightX = 0;
    private float arrowLeftXDest = 0;
    private float arrowRightXDest = 0;
    private float arrowY;
    private float arrowYDest;

    private float playZRot;
    private float playScale;
    private float playScaleBig;
    private float playScaleSmall;

    public int currentChoice = 0;

    [SerializeField] public bool inputLeft = false;
    [SerializeField] public bool inputRight = false;
    [SerializeField] public bool inputUp = false;
    [SerializeField] public bool inputDown = false;

    void Start()
    {
        openingMenuObj = GameObject.Find("GameTitle");
        levelSettingsObj = GameObject.Find("LevelSettingsUI");
        transitionManager = GameObject.Find("SceneTransition");

        playObj = GameObject.Find("FirstMenu_Play");
        creditsObj = GameObject.Find("FirstMenu_Credits");
        arrowLeftObj = GameObject.Find("FirstMenu_ArrowLeft");
        arrowRightObj = GameObject.Find("FirstMenu_ArrowRight");
        quitObj = GameObject.Find("FirstMenu_Quit");

        menuItems[0] = playObj;
        menuItems[1] = creditsObj;
        menuItems[2] = quitObj;

        playScaleBig = 1;
        playScaleSmall = 0.7f;
    }

    void Update()
    {
        bool showFirstMenu = (openingMenuObj.GetComponent<OpeningMenu>().menuState == 1);
        float ySpacing = Screen.height / 7;
        float plusYFull = -Screen.height;
        float plusY = (showFirstMenu) ? 0 : plusYFull;
        canContinue = Mathf.Abs(plusY - plusYFull) > 1;

        if (!showFirstMenu) {
            currentChoice = 0;
        }
    
        for (int i = 0; i < menuItems.Length; i++) {

            GameObject currentMenuObj = menuItems[i];

            // set position
            float currentYDest = (Screen.height * 0.85f) - (i * ySpacing) + plusY;
            float currentY = Damp(currentMenuObj.transform.position.y, currentYDest, 18f, Time.deltaTime);
            currentMenuObj.transform.position = new Vector3(
                Screen.width / 2,
                currentY,
                0
            );

            // hover effect
            if (currentChoice == i) {

                hoveringOnPlay = (currentMenuObj == playObj);
                hoveringOnCredits = (currentMenuObj == creditsObj);
                hoveringOnQuit= (currentMenuObj == quitObj);

                arrowYDest = currentY;
                float screenWidthRatio = 0;
                if (hoveringOnPlay) {
                    screenWidthRatio = 0.3f;
                }
                else if (hoveringOnCredits) {
                    screenWidthRatio = 0.32f;
                }
                else if (hoveringOnQuit) {
                    screenWidthRatio = 0.35f;
                }
                arrowLeftXDest = Screen.width * screenWidthRatio;
                arrowRightXDest = Screen.width * (1 - screenWidthRatio);
            }

            Color colorSelected = levelSettingsObj.GetComponent<LevelSettingsNew>().colorSelected;
            Color colorUnselected = levelSettingsObj.GetComponent<LevelSettingsNew>().colorUnselected;
            // set color for text
            currentMenuObj.GetComponent<Text>().color = (currentChoice == i) ? colorSelected : colorUnselected;
        }


        // set position for arrows
        arrowLeftX = Damp(arrowLeftX, arrowLeftXDest, 18f, Time.deltaTime);
        arrowRightX = Damp(arrowRightX, arrowRightXDest, 18f, Time.deltaTime);
        arrowY = Damp(arrowY, arrowYDest, 18f, Time.deltaTime);

        // bob arrows
        float arrowBobber = (Mathf.Sin(Time.time * 8) * 8);
        float arrowLeftXDestNew = arrowLeftX + arrowBobber;
        float arrowRightXDestNew = arrowRightX - arrowBobber;
        float arrowLeftXNew = Damp(arrowLeftObj.transform.position.x, arrowLeftXDestNew, 18f, Time.deltaTime);
        float arrowRightXNew = Damp(arrowRightObj.transform.position.x, arrowRightXDestNew, 18f, Time.deltaTime);
        arrowLeftObj.transform.position = new Vector3(arrowLeftXNew, arrowY, 0);
        arrowRightObj.transform.position = new Vector3(arrowRightXNew, arrowY, 0);


        // fun rotation for PLAY!
        playZRot = (hoveringOnPlay) ? (Mathf.Sin(Time.time * 8) * 2f) : Damp(playZRot, 0, 18f, Time.deltaTime);
        playObj.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, playZRot);
        float playScaleDest = (hoveringOnPlay) ? playScaleBig : playScaleSmall;
        playScale = Damp(playScale, playScaleDest, 18f, Time.deltaTime);
        playObj.GetComponent<RectTransform>().localScale = new Vector3(playScale, playScale, 1);




        if (inputUp) {
            if (inputTimer) {
                inputTimer = false;
                InvokeResetInputTimer();
                currentChoice--;
                if (currentChoice < 0) {
                    currentChoice = menuItems.Length - 1;
                }
            }
        }
        if (inputDown) {
            if (inputTimer) {
                inputTimer = false;
                InvokeResetInputTimer();
                currentChoice++;
                if (currentChoice > menuItems.Length - 1) {
                    currentChoice = 0;
                }
            }
        }

        Vector3 arrowScale = (levelSettingsObj.GetComponent<LevelSettingsNew>()).arrowLeftObj.GetComponent<RectTransform>().localScale;
        arrowLeftObj.GetComponent<RectTransform>().localScale = arrowScale;
        arrowRightObj.GetComponent<RectTransform>().localScale = arrowScale;


    }

    //New input system 2-Axis selection
    public void updateDir(Vector2 data)
    {
        if (openingMenuObj.GetComponent<OpeningMenu>().menuState == 1)
        {
            //Left
            if (data.x <= -0.9f)
            {
                inputLeft = true;
                return;
            }
            //Right
            else if (data.x >= 0.9f)
            {
                inputRight = true;
                return;
            }
            //Up
            else if (data.y >= 0.9f)
            {
                inputUp = true;
                return;
            }
            //Down
            else if (data.y <= -0.9f)
            {
                inputDown = true;
                return;
            }
            else
            {
                inputLeft = false;
                inputRight = false;
                inputUp = false;
                inputDown = false;
                return;
            }
        }
    }

    public void Continue()
    {
        if (inputTimer && canContinue) {
            if (hoveringOnPlay) {
                //Debug.Log("Continuing to menuState 2");
                openingMenuObj.GetComponent<OpeningMenu>().menuState = 2;
            }
            else if (hoveringOnCredits) {
                Debug.Log("Continuing to Credits");
                transitionManager.GetComponent<TransitionManagerMenu>().CreditsClose();
            }
            else if (hoveringOnQuit) {
                Debug.Log("Quitting game");
                Application.Quit();
            }
        }
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    public void InvokeResetInputTimer()
    {
        Invoke("ResetInputTimer", inputDelayTime);
    }

    public void ResetInputTimer()
    {
        inputTimer = true;
    }
}