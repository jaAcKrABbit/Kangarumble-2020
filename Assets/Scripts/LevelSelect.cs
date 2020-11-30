using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public string[] levelNames;

    public int levelAmount = 0;
    public int levelCurrent = 0;

    private bool inputLeft = false;
    private bool inputRight = false;

    [SerializeField]
    private GameObject playerAmountObj;
    private GameObject levelNameTextObj;
    public float levelNameTextXDest = 0;
    public float levelNameTextX = 0;
    public bool levelNameTextXGoingLeft = false;
    public bool levelNameTextXGoingRight = false;
    public bool levelNameTextXComing = true;
    public float levelNameTextXDrift = 40;

    public Color levelNameVisibleColor;
    public Color levelNameInvisibleColor;
    private Color levelNameCurrentColor;
    private Color levelNameCurrentColorTarget;

    public GameObject[] bgObjs = new GameObject[5];
    public float bgOffset = 18;
    public float[] bgXDest = new float[5];

    private GameObject openingMenuControlObj;
    private GameObject levelSettingsObj;
    private bool inputAdvance;


    private GameObject arrowLeft;
    private GameObject arrowRight;
    public float arrowDestXPlus = 300;
    public float arrowDestAlpha = 0;

    private GameObject playerSelectObj;

    public GameObject[] levelMarkerObjs;
    public Sprite levelMarkerSpriteSelected;
    public Sprite levelMarkerSpriteUnSelected;
    public float levelMarkerScaleSelected;
    public float levelMarkerScaleUnSelected;
    public float[] levelMarkerScale = new float [5];

    public float dampSmoothRate = 0.05f;

    public bool inputTimer;
    public float inputDelayTime;


    void Start()
    {
        levelAmount = levelNames.Length;
        levelNameTextObj = GameObject.Find("LevelName");
        levelNameTextObj.GetComponent<Text>().text = levelNames[levelCurrent];
        levelNameCurrentColor = levelNameVisibleColor;

        openingMenuControlObj = GameObject.Find("GameTitle");

        levelNameTextObj.SetActive(false);

        // set initial background X positions
        bgXDest[0] = -bgOffset;
        bgXDest[1] = 0;
        bgXDest[2] = bgOffset;
        bgXDest[3] = bgOffset;
        bgXDest[4] = bgOffset;


        // set alpha to arrows initially to 0
        arrowLeft = GameObject.Find("ArrowLeft");
        arrowRight = GameObject.Find("ArrowRight");
        arrowLeft.GetComponent<Image>().color = new Vector4(255, 255, 255, 0);
        arrowRight.GetComponent<Image>().color = arrowLeft.GetComponent<Image>().color;

        playerSelectObj = GameObject.Find("GameTitle");
        levelSettingsObj = GameObject.Find("LevelSettingsUI");
    }

    void Update()
    {
        if (openingMenuControlObj.GetComponent<OpeningMenu>().menuState != 3){
            inputTimer = false;
        }
        // set initial positions for arrows for this frame
        float arrowBobber = (Mathf.Sin(Time.time * 8) * 12f);
        float arrowLeftXDest = (Screen.width * 0.14f) - arrowBobber;
        float arrowRightXDest = (Screen.width * (1 - 0.14f)) + arrowBobber;
        float arrowLeftX = Damp(arrowLeft.GetComponent<Transform>().position.x, arrowLeftXDest, dampSmoothRate, Time.deltaTime);
        float arrowRightX = Damp(arrowRight.GetComponent<Transform>().position.x, arrowRightXDest, dampSmoothRate, Time.deltaTime);

        if (openingMenuControlObj.GetComponent<OpeningMenu>().menuState != 0 && openingMenuControlObj.GetComponent<OpeningMenu>().menuState != 1
        && openingMenuControlObj.GetComponent<OpeningMenu>().menuState != 2 && openingMenuControlObj.GetComponent<OpeningMenu>().menuState != 4) {
            levelNameTextObj.SetActive(true);
            arrowDestAlpha = 1;
        }
        else {
            arrowDestAlpha = 0;
        }

        // use left and right inputs to cycle through level choices
        if (inputTimer && inputLeft && Mathf.Abs(levelNameTextX - levelNameTextXDest) < 2
        && openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 3)
        {
            levelCurrent--;
            levelNameTextXComing = false;
            levelNameTextXGoingLeft = true;
            levelNameTextXGoingRight = false;
            arrowLeftX -= 60;
            inputTimer = false;
            Invoke("ResetInputTimer", inputDelayTime);
        }
        if (inputTimer && inputRight && Mathf.Abs(levelNameTextX - levelNameTextXDest) < 2
        && openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 3)
        {
            levelCurrent++;
            levelNameTextXComing = false;
            levelNameTextXGoingLeft = false;
            levelNameTextXGoingRight = true;
            arrowRightX += 60;
            inputTimer = false;
            Invoke("ResetInputTimer", inputDelayTime);
        }
        //left right logic in callbacks

        if (levelCurrent < 0) {
            levelCurrent = levelAmount - 1;
        }
        if (levelCurrent >= levelAmount) {
            levelCurrent = 0;
        }

        if (openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 3) {
            // change text and position of currentLevelName object
            if (levelNameTextXComing) {
                levelNameCurrentColorTarget = levelNameVisibleColor;
                levelNameTextXDest = Screen.width / 2;
            }
            if (levelNameTextXGoingLeft) {
                levelNameCurrentColorTarget = levelNameInvisibleColor;
                levelNameTextXDest = (Screen.width / 2) - levelNameTextXDrift;
                if (Mathf.Abs(levelNameTextX - levelNameTextXDest) < 2) {
                    levelNameTextX = (Screen.width / 2) + levelNameTextXDrift;
                    levelNameTextXGoingLeft = false;
                    levelNameTextXComing = true;
                    levelNameTextObj.GetComponent<Text>().text = levelNames[levelCurrent];
                }
            }
            if (levelNameTextXGoingRight) {
                levelNameCurrentColorTarget = levelNameInvisibleColor;
                levelNameTextXDest = (Screen.width / 2) + levelNameTextXDrift;
                if (Mathf.Abs(levelNameTextX - levelNameTextXDest) < 2) {
                    levelNameTextX = (Screen.width / 2) - levelNameTextXDrift;
                    levelNameTextXGoingRight = false;
                    levelNameTextXComing = true;
                    levelNameTextObj.GetComponent<Text>().text = levelNames[levelCurrent];
                }
            }
        }
        else {
            levelNameCurrentColorTarget = levelNameInvisibleColor;
            levelNameTextXDest = (Screen.width / 2) - levelNameTextXDrift;
            levelNameTextX = levelNameTextXDest;
        }

        levelNameTextX = Damp(levelNameTextX, levelNameTextXDest, dampSmoothRate, Time.deltaTime);
        levelNameTextObj.transform.position = new Vector3(levelNameTextX, Screen.height / 2, 0);


        SetBackgroundPositions();


        // change opactity/alpha of level name text
        levelNameCurrentColor = ColorDamp(levelNameCurrentColor, levelNameCurrentColorTarget, dampSmoothRate * 2, Time.deltaTime);
        levelNameTextObj.GetComponent<Text>().color = levelNameCurrentColor;
        Color effectColor = new Color(0, 0, 0, levelNameCurrentColor.a);
        levelNameTextObj.GetComponent<Shadow>().effectColor = effectColor;
        levelNameTextObj.GetComponent<Outline>().effectColor = effectColor;


        // change opacity & position of menu arrows
        float arrowAlpha = Damp(arrowLeft.GetComponent<Image>().color.a, arrowDestAlpha, dampSmoothRate, Time.deltaTime);
        arrowLeft.GetComponent<Image>().color = new Vector4(255, 255, 255, arrowAlpha);
        arrowRight.GetComponent<Image>().color = arrowLeft.GetComponent<Image>().color;
        arrowLeft.GetComponent<Transform>().position = new Vector3(
            arrowLeftX,
            arrowLeft.GetComponent<Transform>().position.y,
            arrowLeft.GetComponent<Transform>().position.z
        );
        arrowRight.GetComponent<Transform>().position = new Vector3(
            arrowRightX,
            arrowRight.GetComponent<Transform>().position.y,
            arrowRight.GetComponent<Transform>().position.z
        );



        // change image, position, and scale of level markers
        float levelMarkerXSpacing = (Screen.width) / 16;
        for (int i = 0; i < 5; i++) {
            GameObject currentLevelMarker = levelMarkerObjs[i];
            currentLevelMarker.GetComponent<Image>().sprite = (levelCurrent == i) ? levelMarkerSpriteSelected : levelMarkerSpriteUnSelected;
            currentLevelMarker.GetComponent<Transform>().position = new Vector3(
                levelMarkerXSpacing * (i + 6),
                Screen.height * 0.7f,
                0
            );
            float levelMarkerScaleDest = 0;
            if (openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 3) {
                levelMarkerScaleDest = (levelCurrent == i) ? levelMarkerScaleSelected : levelMarkerScaleUnSelected;
            }
            levelMarkerScale[i] = Damp(levelMarkerScale[i], levelMarkerScaleDest, dampSmoothRate, Time.deltaTime);
            currentLevelMarker.GetComponent<Transform>().localScale = new Vector3(levelMarkerScale[i], levelMarkerScale[i], levelMarkerScale[i]);
        }

        //TODO: Return from levelSelect now in callbacks
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    public static Color ColorDamp(Color colorA, Color colorB, float lambda, float dt)
    {
        float r = Damp(colorA.r, colorB.r, lambda, dt);
        float g = Damp(colorA.g, colorB.g, lambda, dt);
        float b = Damp(colorA.b, colorB.b, lambda, dt);
        float a = Damp(colorA.a, colorB.a, lambda, dt);
        return new Color(r, g, b, a);
    }

    public void InvokeTimer()
    {
        Invoke("ResetInputTimer", inputDelayTime);
    }

    public void ResetInputTimer()
    {
        inputTimer = true;
    }

    //New input system left/right selection
    public void updateDir(float horizontal)
    {
        //Debug.Log(horizontal);
        if (openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 3)
        {
            //Left
            if (horizontal <= -0.9f)
            {
                inputLeft = true;
                return;
            }
            //Right
            else if (horizontal >= 0.9f)
            {
                inputRight = true;
                return;
            }
            else
            {
                inputLeft = false;
                inputRight = false;
                return;
            }
        }
    }

    //New input system continue to level Settings
    public void Continue()
    {
        //take you to level settings
        if (inputTimer && openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 3
        && Mathf.Abs(levelNameTextX - levelNameTextXDest) < 2)
        {
            levelSettingsObj.GetComponent<LevelSettingsNew>().levelCurrent = levelCurrent;
            openingMenuControlObj.GetComponent<OpeningMenu>().menuState = 4;
            levelSettingsObj.GetComponent<LevelSettingsNew>().inputTimer = false;
            levelSettingsObj.GetComponent<LevelSettingsNew>().Invoke("ResetInputTimer", 0.5f);
        }
    }

    //New input system gamepad Back 
    public void Back()
    {
        //take you to level settings
        if (inputTimer && openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 3
        && Mathf.Abs(levelNameTextX - levelNameTextXDest) < 2)
        {
            openingMenuControlObj.GetComponent<OpeningMenu>().menuState = 2;
            
        }
    }

    public void SetBackgroundPositions()
    {
        // set X positions for backgrounds
        for (int i = 0; i < bgObjs.Length; i++) {
            GameObject currentBG = bgObjs[i];
            float currentBGX = currentBG.transform.position.x;
            if (Mathf.Abs(currentBGX - bgXDest[i]) > bgOffset + 3) {
                currentBGX = bgXDest[i];
            }
            else {
                currentBGX = Damp(currentBGX, bgXDest[i], dampSmoothRate, Time.deltaTime);
            }
            float currentBGZ = (levelCurrent == i) ? -3 : 0;
            currentBG.transform.position = new Vector3(currentBGX, 0, currentBGZ);
        }


        int bgOnLeft = levelCurrent - 1;
        int bgOnRight = levelCurrent + 1;

        bgOnLeft = (bgOnLeft < 0) ? bgObjs.Length - 1 : bgOnLeft;
        if (bgOnRight > 4) {
            bgOnRight = 0;
        }

        bgXDest[levelCurrent] = 0;
        bgXDest[bgOnLeft] = -bgOffset;
        bgXDest[bgOnRight] = bgOffset;
    }
}
