using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSettingsNew : MonoBehaviour
{
    public bool inputTimer;
    public float inputDelayTime;

    private GameObject openingMenuObj;
    private GameObject transitionManager;
    private GameObject levelSelectObj;
    private GameObject playerAmountObj;
    private GameObject turnLengthObj;
    private GameObject scoreLimitObj;
    private GameObject playObj;
    private GameObject currentChoiceUILeftObj;
    private GameObject currentChoiceUIRightObj;
    public GameObject arrowLeftObj;
    public GameObject arrowRightObj;

    public int levelCurrent = 0;

    private float playZRot;
    private float playScale;
    private float playScaleBig;
    private float playScaleSmall;
    public float arrowScale;
    public Sprite arrowLeftSprite;
    public Sprite arrowRightSprite;
    private float arrowScaleOriginal;
    public Color colorUnselected;
    public Color colorSelected;

    public GameObject[] levelSettingsObj = new GameObject[3];
    public int currentChoice = 0;

    public int currentTurnLength = 1;
    public int currentScoreLimit = 10;
    public int turnLengthMin = 0;
    public int turnLengthMax = 2;
    public int scoreLimitMin = 1;
    public int scoreLimitMax = 20;

    [SerializeField] public bool inputLeft = false;
    [SerializeField] public bool inputRight = false;
    [SerializeField] public bool inputUp = false;
    [SerializeField] public bool inputDown = false;

    private float currentChoiceUILeftX;
    private float currentChoiceUIRightX;
    private float currentChoiceUILeftXDest;
    private float currentChoiceUIRightXDest;
    private float currentChoiceUIY;
    private float currentChoiceUIYDest;

    public float timeLimitMin = 0;
    public float timeLimitMax = 0;
    public float timeLimitDec = 0;

    private bool hoveringOnPlay;
    private bool hoveringOnScoreLimit;
    private bool hoveringOnTurnLength;

    void Start()
    {
        openingMenuObj = GameObject.Find("GameTitle");
        transitionManager = GameObject.Find("SceneTransition");
        levelSelectObj = GameObject.Find("LevelSelect");
        playerAmountObj = GameObject.Find("PlayerAmountObj");

        // load level settings objects into array
        turnLengthObj = GameObject.Find("LevelSettings_TurnLength");
        scoreLimitObj = GameObject.Find("LevelSettings_ScoreLimit");
        playObj = GameObject.Find("LevelSettings_Play");
        levelSettingsObj[0] = playObj;
        levelSettingsObj[1] = scoreLimitObj;
        levelSettingsObj[2] = turnLengthObj;

        currentChoiceUILeftObj = GameObject.Find("LevelSettings_CurrentChoiceUILeft");
        currentChoiceUIRightObj = GameObject.Find("LevelSettings_CurrentChoiceUIRight");

        arrowLeftObj = GameObject.Find("LevelSettings_ArrowLeft");
        arrowRightObj = GameObject.Find("LevelSettings_ArrowRight");
        arrowScaleOriginal = arrowScale;

        playScaleBig = 1;
        playScaleSmall = 0.75f;

        hoveringOnPlay = false;
        hoveringOnScoreLimit = false;
        hoveringOnTurnLength = false;
        inputTimer = false;
    }

    void Update()
    {
        bool levelSettingsActive = (openingMenuObj.GetComponent<OpeningMenu>().menuState == 4);
        float ySpacing = Screen.height / 8;
        float plusYFull = -Screen.height;
        float plusY = (levelSettingsActive) ? 0 : plusYFull;

        if (!levelSettingsActive) {
            currentChoice = 0;
        }

        for (int i = 0; i < 3; i++) {

            GameObject currentLevelSettingsObj = levelSettingsObj[i];

            // set position for level settings
            float currentYDest = (Screen.height * 0.85f) - (i * ySpacing) + plusY;
            float currentY = Damp(currentLevelSettingsObj.transform.position.y, currentYDest, 18f, Time.deltaTime);
            currentLevelSettingsObj.transform.position = new Vector3(
                Screen.width / 2,
                currentY,
                0
            );

            // set dest position for currentChoiceUI
            if (currentChoice == i) {

                hoveringOnPlay = (currentLevelSettingsObj == playObj);
                hoveringOnTurnLength = (currentLevelSettingsObj == turnLengthObj);
                hoveringOnScoreLimit = (currentLevelSettingsObj == scoreLimitObj);

                currentChoiceUIYDest = currentY;
                float screenWidthRatio = 0;
                if (hoveringOnPlay) {
                    screenWidthRatio = 0.3f;
                }
                else if (hoveringOnTurnLength) {
                    screenWidthRatio = 0.2f;
                }
                else if (hoveringOnScoreLimit) {
                    screenWidthRatio = 0.275f;
                }
                currentChoiceUILeftXDest = Screen.width * screenWidthRatio;
                currentChoiceUIRightXDest = Screen.width * (1 - screenWidthRatio);
            }

            // set color for text
            currentLevelSettingsObj.GetComponent<Text>().color = (currentChoice == i) ? colorSelected : colorUnselected;
        }

        // set position for currentChoiceUI
        currentChoiceUILeftX = Damp(currentChoiceUILeftX, currentChoiceUILeftXDest, 18f, Time.deltaTime);
        currentChoiceUIRightX = Damp(currentChoiceUIRightX, currentChoiceUIRightXDest, 18f, Time.deltaTime);
        currentChoiceUIY = Damp(currentChoiceUIY, currentChoiceUIYDest, 18f, Time.deltaTime);
    
        currentChoiceUILeftObj.transform.position = new Vector3(
            currentChoiceUILeftX,
            currentChoiceUIY,
            0
        );
        currentChoiceUIRightObj.transform.position = new Vector3(
            currentChoiceUIRightX,
            currentChoiceUIY,
            0
        );
        float arrowBobber = (Mathf.Sin(Time.time * 8) * 8f);
        float arrowLeftXDest = (currentChoiceUILeftX) - arrowBobber;
        float arrowRightXDest = (currentChoiceUIRightX) + arrowBobber;
        float arrowLeftX = Damp(arrowLeftObj.GetComponent<Transform>().position.x, arrowLeftXDest, 18f, Time.deltaTime);
        float arrowRightX = Damp(arrowRightObj.GetComponent<Transform>().position.x, arrowRightXDest, 18f, Time.deltaTime);
        arrowLeftObj.transform.position = new Vector3(arrowLeftX, currentChoiceUIY, 0);
        arrowRightObj.transform.position = new Vector3(arrowRightX, currentChoiceUIY, 0);


        playZRot = (hoveringOnPlay) ? (Mathf.Sin(Time.time * 8) * 2f) : Damp(playZRot, 0, 18f, Time.deltaTime);
        playObj.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, playZRot);
        float playScaleDest = (hoveringOnPlay) ? playScaleBig : playScaleSmall;
        playScale = Damp(playScale, playScaleDest, 18f, Time.deltaTime);
        playObj.GetComponent<RectTransform>().localScale = new Vector3(playScale, playScale, 1);





        if (inputUp) {
            if (inputTimer) {
                inputTimer = false;
                Invoke("ResetInputTimer", inputDelayTime);
                currentChoice--;
                if (currentChoice < 0) {
                    currentChoice = levelSettingsObj.Length - 1;
                }
            }
        }
        if (inputDown) {
            if (inputTimer) {
                inputTimer = false;
                Invoke("ResetInputTimer", inputDelayTime);
                currentChoice++;
                if (currentChoice > levelSettingsObj.Length - 1) {
                    currentChoice = 0;
                }
            }
        }
        if (inputLeft && !hoveringOnPlay) {
            if (inputTimer) {
                inputTimer = false;
                Invoke("ResetInputTimer", inputDelayTime);
                if (hoveringOnTurnLength) {
                    // change turnLength left
                    currentTurnLength--;
                }
                else if (hoveringOnScoreLimit) {
                    // change score limit left
                    currentScoreLimit--;
                }
            }
        }
        if (inputRight && !hoveringOnPlay) {
            if (inputTimer) {
                inputTimer = false;
                Invoke("ResetInputTimer", inputDelayTime);
                if (hoveringOnTurnLength) {
                    // change turnLength left
                    currentTurnLength++;
                }
                else if (hoveringOnScoreLimit) {
                    // change score limit left
                    currentScoreLimit++;
                }
            }
        }

        // clamp turnLength and score limit
        if (currentTurnLength < turnLengthMin) {
            currentTurnLength = turnLengthMax;
        }
        if (currentTurnLength > turnLengthMax) {
            currentTurnLength = turnLengthMin;
        }
        if (currentScoreLimit < scoreLimitMin) {
            currentScoreLimit = scoreLimitMax;
        }
        if (currentScoreLimit > scoreLimitMax) {
            currentScoreLimit = scoreLimitMin;
        }

        arrowLeftObj.GetComponent<RectTransform>().localScale = new Vector3(
            arrowScale,
            arrowScale,
            1
        );
        arrowRightObj.GetComponent<RectTransform>().localScale = arrowLeftObj.GetComponent<RectTransform>().localScale;
        arrowLeftObj.GetComponent<Image>().sprite = (hoveringOnPlay) ? arrowRightSprite : arrowLeftSprite;
        arrowRightObj.GetComponent<Image>().sprite = (hoveringOnPlay) ? arrowLeftSprite : arrowRightSprite;



        // set text for level settings
        string turnLengthStr = "";
        if (currentTurnLength == 0) {
            turnLengthStr = "SLOW";
        }
        else if (currentTurnLength == 1) {
            turnLengthStr = "NORMAL";
        }
        else if (currentTurnLength == 2) {
            turnLengthStr = "FAST";
        }
        turnLengthObj.GetComponent<Text>().text = "TURN LENGTH: " + turnLengthStr;
        scoreLimitObj.GetComponent<Text>().text = "SCORE LIMIT: " + currentScoreLimit.ToString();

        UpdateTurnLengthSettings();
    }

    public void UpdateTurnLengthSettings()
    {
        switch (currentTurnLength) {
            case 0:
                timeLimitMin = 2.0f;
                timeLimitMax = 6.0f;
                timeLimitDec = 0.3f;
                break;
            case 1:
                timeLimitMin = 1.5f;
                timeLimitMax = 5.0f;
                timeLimitDec = 0.3f;
                break;
            case 2:
                timeLimitMin = 1.0f;
                timeLimitMax = 4.0f;
                timeLimitDec = 0.4f;
                break;
            default:
                timeLimitMin = 1.0f;
                timeLimitMax = 5.0f;
                timeLimitDec = 0.3f;
                break;
        }
    }


    
    //New input system 2-Axis selection
    public void updateDir(Vector2 data)
    {
        if (openingMenuObj.GetComponent<OpeningMenu>().menuState == 4)
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
        //take you the level
        if (hoveringOnPlay) {
            if (inputTimer && openingMenuObj.GetComponent<OpeningMenu>().menuState == 4)
            {
                Debug.Log("Continue to level");
                levelCurrent = levelSelectObj.GetComponent<LevelSelect>().levelCurrent;

                Debug.Log("Loading level " + levelCurrent + "...");
                string currentLevelName = "";
                if (levelCurrent == 0)
                {
                    currentLevelName = "TestLevel";
                }
                else if (levelCurrent == 1)
                {
                    currentLevelName = "Industrial";
                }
                else if (levelCurrent == 2)
                {
                    currentLevelName = "Carnival";
                }

                //pass level settings data to playerAmountObj.GetComponent<PlayerAmount>()
                playerAmountObj.GetComponent<PlayerAmount>().scoreLimit = currentScoreLimit;
                playerAmountObj.GetComponent<PlayerAmount>().timeLimitMin = timeLimitMin;
                playerAmountObj.GetComponent<PlayerAmount>().timeLimitMax = timeLimitMax;
                playerAmountObj.GetComponent<PlayerAmount>().timeLimitDec = timeLimitDec;
                playerAmountObj.GetComponent<PlayerAmount>().currentLevelName = currentLevelName;

                transitionManager.GetComponent<TransitionManagerMenu>().MenuEndClose();
            }
        }
    }

    //New input system gamepad Back 
    public void Back()
    {
        Debug.Log("LevelSettingsNew Back()");
        if (inputTimer && openingMenuObj.GetComponent<OpeningMenu>().menuState == 4)
        {
            //take you to level settings
            openingMenuObj.GetComponent<OpeningMenu>().menuState = 3;
            levelSelectObj.GetComponent<LevelSelect>().InvokeTimer();
        }
    }

    public void ResetInputTimer()
    {
        inputTimer = true;
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
