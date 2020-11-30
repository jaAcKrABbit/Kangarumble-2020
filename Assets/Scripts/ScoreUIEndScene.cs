using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUIEndScene : MonoBehaviour
{
    private GameObject mainCam;
    private GameObject sceneCam;
    private GameObject playerVarsObj;

    public GameObject[] scoreUIParentObj = new GameObject[4];
    public GameObject[] scoreUITextScoreObj = new GameObject[4];
    public GameObject[] scoreUITextScoreLimitObj = new GameObject[4];
    public GameObject[] scoreUITextNameObj = new GameObject[4];
    public GameObject[] scoreUITextWaitingObj = new GameObject[4];
    public GameObject[] scoreUICloudObj = new GameObject[4];
    public GameObject[] scoreUIAvatarObj = new GameObject[4];
    public GameObject[] scoreUIBackObj = new GameObject[4];

    public Sprite[] avatarSprite = new Sprite[4];

    public float scoreXOffset = -10;
    public float[] scoreXPos = new float[4];
    public float scoreYPosRatio = 0.2f;
    public float scoreYPos = 300;
    public float scoreZPos = 0;
    public float scoreTextScoreXOffsetRatio = 5.05f;
    public float scoreTextScoreXOffset = 100;
    public float scoreTextScoreYOffset = 11;
    public float scoreTextNameXOffsetRatio = 5.05f;
    public float scoreTextNameYOffsetRatio = 5.05f;
    public float scoreTextNameXOffset = 100;
    public float scoreTextNameYOffset = 11;
    public float scoreBackXOffsetRatio = 5.05f;
    public float scoreBackXOffset = 100;
    public float scoreBackYOffset = 11;

    public Color textColorMyTurn;
    public Color textColorNotMyTurn;

    public float cloudScaleBig = 1.5f;
    public float cloudScaleSmall = 1f;
    public float avatarScaleRatio = 0.9f;
    public float scorelimitXRatio = 0.03f;
    public float scorelimitYRatio = 0.03f;
    public float waitingXRatio = 0.03f;
    public float waitingYRatio = 0.03f;

    private int playerAmount = 0;
    private float[] scoreUICloudScale = new float[4];
    private float[] scoreUICloudScaleDest = new float[4];

    public Color[] scoreUICloudColor = new Color[4];

    //public int[] playerOrder;
    public int[] charSelect;

    public float countdownPlusY = -200;

    public bool[] playerIn = new bool[4];
    public GameObject continueController;
    public GameObject continueKeyboard;
    public float continueXRatio;
    public float continueYRatio;
    private float continueControllerX;
    private float continueKeyboardX;
    private float continueY;

    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        sceneCam = GameObject.Find("SceneCamera");
        playerVarsObj = GameObject.Find("PlayerVars");

        // set colors for score UI clouds
        for (int i = 0; i < 4; i++) {
            scoreUICloudObj[i].GetComponent<Image>().color = scoreUICloudColor[i];
        }

        // get player order array from PlayerVars object
        //playerOrder = playerVarsObj.GetComponent<PlayerVars>().playerOrder;
        charSelect = playerVarsObj.GetComponent<PlayerVars>().charSelect;

        playerVarsObj.GetComponent<PlayerVars>().playerIn.CopyTo(playerIn, 0);
        continueControllerX = -(Screen.width * continueXRatio);
        continueKeyboardX = (Screen.width + (Screen.width * continueXRatio));
        continueY = Screen.height * continueYRatio;
        continueKeyboard.transform.position = new Vector3(continueControllerX, continueY, 0);
        continueKeyboard.transform.position = new Vector3(continueKeyboardX, continueY, 0);

        DeactivateUnusedPlayerUI();

        Invoke("SetScoreLimitText", 0.25f);
    }

    void Update()
    {
        countdownPlusY = Damp(countdownPlusY, 0, 12f, Time.deltaTime);
        if (Mathf.Abs(countdownPlusY) < 0.001) {
            countdownPlusY = 0;
        }

        // set ratios for X positions of score UI elements
        scoreTextScoreXOffset = Screen.width / scoreTextScoreXOffsetRatio;
        scoreTextNameXOffset = Screen.width / scoreTextNameXOffsetRatio;
        scoreTextNameYOffset = Screen.height / scoreTextNameYOffsetRatio;
        scoreBackXOffset = Screen.width / scoreBackXOffsetRatio;

        DeactivateUnusedPlayerUI();

        scoreYPos = Screen.height * scoreYPosRatio;


        float scoreXSpacing = Screen.width / (playerAmount + 1);
        int iActive = -1;
        for (int i = 0; i < 4; i++) {

            if (!playerIn[i]) {
                continue;
            }
            iActive++;

            GameObject currentScoreTextScoreObj = scoreUITextScoreObj[i];
            GameObject currentScoreTextScoreLimitObj = scoreUITextScoreLimitObj[i];
            GameObject currentScoreTextNameObj = scoreUITextNameObj[i];
            GameObject currentScoreCloudObj = scoreUICloudObj[i];
            GameObject currentAvatarObj = scoreUIAvatarObj[i];
            GameObject currentScoreBackObj = scoreUIBackObj[i];
            GameObject currentWaitingObj = scoreUITextWaitingObj[i];


            // set positions of score UI text, avatar, and cloud
            scoreXPos[i] = scoreXSpacing * (iActive + 1) + scoreXOffset;
            currentScoreCloudObj.transform.position = new Vector3(scoreXPos[i], scoreYPos + countdownPlusY, scoreZPos);
            currentAvatarObj.transform.position = currentScoreCloudObj.transform.position;
            currentScoreTextScoreObj.transform.position = new Vector3(scoreXPos[i] + scoreTextScoreXOffset, scoreYPos + scoreTextScoreYOffset + countdownPlusY, scoreZPos);
            currentScoreTextScoreLimitObj.transform.position = new Vector3(currentScoreTextScoreObj.transform.position.x + (Screen.width * scorelimitXRatio), currentScoreTextScoreObj.transform.position.y + (Screen.height * scorelimitYRatio), currentScoreTextScoreObj.transform.position.z);
            currentScoreTextNameObj.transform.position = new Vector3(scoreXPos[i] + scoreTextNameXOffset, scoreYPos + scoreTextNameYOffset + countdownPlusY, scoreZPos);
            currentScoreBackObj.transform.position = new Vector3(scoreXPos[i] + scoreBackXOffset, scoreYPos + scoreBackYOffset + countdownPlusY, scoreZPos);
            currentWaitingObj.transform.position = new Vector3(currentScoreTextScoreObj.transform.position.x + (Screen.width * waitingXRatio), currentScoreTextScoreObj.transform.position.y + (Screen.height * waitingYRatio), currentScoreTextScoreObj.transform.position.z);

            // set text for score UI text
            currentScoreTextScoreObj.GetComponent<Text>().text = (playerVarsObj.GetComponent<PlayerVars>().playerScores[i]).ToString();
            currentScoreTextNameObj.GetComponent<Text>().text = (playerVarsObj.GetComponent<PlayerVars>().playerNames[charSelect[i]]).ToString();

            // set avatar sprite
            currentAvatarObj.GetComponent<Image>().sprite = avatarSprite[charSelect[i]];

            scoreUICloudScaleDest[i] = cloudScaleSmall;
            currentScoreTextScoreObj.GetComponent<Text>().color = textColorNotMyTurn;
            currentScoreTextNameObj.GetComponent<Text>().color = textColorNotMyTurn;

            scoreUICloudScale[i] = Damp(scoreUICloudScale[i], scoreUICloudScaleDest[i], 14f, Time.deltaTime);
            currentScoreCloudObj.transform.localScale = new Vector3(scoreUICloudScale[i], scoreUICloudScale[i], 1);

            // scale avatar
            currentAvatarObj.transform.localScale = (currentScoreCloudObj.transform.localScale * avatarScaleRatio);
        }

        float continueControllerXDest = Screen.width * continueXRatio;
        float continueKeyboardXDest = Screen.width * (1 - continueXRatio);
        continueControllerX = Damp(continueControllerX, continueControllerXDest, 15f, Time.deltaTime);
        continueKeyboardX = Damp(continueKeyboardX, continueKeyboardXDest, 15f, Time.deltaTime);
        continueY = Screen.height * continueYRatio;
        continueController.transform.position = new Vector3(continueControllerX, continueY, 0);
        continueKeyboard.transform.position = new Vector3(continueKeyboardX, continueY, 0);
    }

    
    void DeactivateUnusedPlayerUI()
    {
        // make sure we are not showing score UI for deactivated players
        playerAmount = playerVarsObj.GetComponent<PlayerVars>().playerAmount;

        for (var i = 0; i < 4; i++) {
            if (!playerIn[i] && scoreUITextScoreObj[i].activeInHierarchy) {
                scoreUITextScoreObj[i].SetActive(false);
                scoreUITextNameObj[i].SetActive(false);
                scoreUICloudObj[i].SetActive(false);
                scoreUIBackObj[i].SetActive(false);
                scoreUIAvatarObj[i].SetActive(false);
                scoreUITextScoreLimitObj[i].SetActive(false);
            }
        }
    }

    void SetScoreLimitText()
    {
        int scoreLimit = playerVarsObj.GetComponent<PlayerVars>().scoreLimit;
        for (int i = 0; i < 4; i++) {
            GameObject currentScoreTextScoreLimitObj = scoreUITextScoreLimitObj[i];
            currentScoreTextScoreLimitObj.GetComponent<Text>().text = "/ " + scoreLimit;
        }
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
