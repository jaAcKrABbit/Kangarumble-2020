using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialImageDisplay : MonoBehaviour
{
    //Possible tutorial imageSets
    public GameObject[] tImageSets1;

    private GameObject loadingTextObj;
    private GameObject tipTextObj;

    private float loadingTextScale;
    private float loadingTextScaleDest;
    private bool fadeOutTipText;

    public int tutorialSet = 1;
    private GameObject playerAmountObj;

    public float smoothDampRateColor;
    public float smoothDampRatePosition;

    [SerializeField] private Color newColor;
    [SerializeField] private Vector3 newPosition;

    public float dest;
    public string startDampingColor;
    public string startDampingPosition;

    public bool debugSkip;

    void Awake()
    {
        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
        GameObject inputManager = GameObject.FindGameObjectWithTag("menuInput");

        if (activePlayers.Length != 0)
        {
            foreach (GameObject player in activePlayers)
            {
                player.GetComponent<playerInputList>().UpdateScenedata();
            }
        }
        inputManager.GetComponent<MenuInputUserControls>().UpdateScenedata();
    }

    void Start()
    {
        playerAmountObj = GameObject.Find("PlayerAmountObj");
        loadingTextObj = GameObject.Find("LoadingText");
        tipTextObj = GameObject.Find("Text");

        if (tutorialSet == 1)
            FirstTutorial();

        newColor.r = 1f;
        newColor.g = 1f;
        newColor.b = 1f;
        newColor.a = 0f;

        loadingTextScale = 0;
        loadingTextScaleDest = 1;
        fadeOutTipText = false;

        debugSkip = false;
    }
    void Update()
    {
        switch (startDampingColor)
        {
            case "1_1":
                UpdateColor("1_1");
                break;
            case "1_2":
                UpdateColor("1_2");
                break;
            case "1_3":
                UpdateColor("1_3");
                break;
            default:
                break;
        }
        switch (startDampingPosition)
            {
                case "1_1":
                    UpdatePosition("1_1");
                    break;
                case "1_2":
                    UpdatePosition("1_2");
                    break;
                case "1_3":
                    UpdatePosition("1_3");
                    break;
                default:
                    break;
            }


        loadingTextScale = Damp(loadingTextScale, loadingTextScaleDest, 9f, Time.deltaTime);
        loadingTextObj.GetComponent<RectTransform>().localScale = new Vector3(loadingTextScale, loadingTextScale, 1);
        if (loadingTextScaleDest == 0) {
            float currentAlpha = tImageSets1[0].GetComponent<Image>().color.a;
            float newAlpha = Damp(currentAlpha, 0, 12f, Time.deltaTime);
            for (int i = 0; i < tImageSets1.Length; i++) {
                tImageSets1[i].GetComponent<Image>().color = new Color(
                    tImageSets1[i].GetComponent<Image>().color.r,
                    tImageSets1[i].GetComponent<Image>().color.g,
                    tImageSets1[i].GetComponent<Image>().color.b,
                    newAlpha
                );
            }

            if (!fadeOutTipText) {
                fadeOutTipText = true;
                tipTextObj.GetComponent<LoadingTextTip>().SetCrossFadeAlphaDown();
            }
        }


        if (debugSkip) {
            takeToLevel();
        }
    }

    void UpdateColor(string targetObj)
    {
        newColor.a = Damp(newColor.a, 1f, smoothDampRateColor, Time.deltaTime);
        if(newColor.a >= 0.999f)
        {
            newColor.a = 1f;
        }
        switch (targetObj)
        {
            case "1_1":
                tImageSets1[0].GetComponent<Image>().color = newColor;
                break;
            case "1_2":
                tImageSets1[1].GetComponent<Image>().color = newColor;
                break;
            case "1_3":
                tImageSets1[2].GetComponent<Image>().color = newColor;
                break;
            default:
                break;
        }
        //reset while done
        if (newColor.a == 1f)
        {
            newColor.a = 0f;
            startDampingColor = "0";
        }
    }

    void UpdatePosition(string targetObj)
    {
        newPosition.x = Damp(newPosition.x, dest, smoothDampRatePosition, Time.deltaTime);
        if (newPosition.x <= dest+0.001)
        {
            newPosition.x = dest;
        }
        switch (targetObj)
        {
            case "1_1":
                tImageSets1[0].GetComponent<Transform>().localPosition = newPosition;
                break;
            case "1_2":
                tImageSets1[1].GetComponent<Transform>().localPosition = newPosition;
                break;
            case "1_3":
                tImageSets1[2].GetComponent<Transform>().localPosition = newPosition;
                break;
            default:
                break;
        }
        //reset while done
        if (newPosition.x == dest)
        {
            dest = 0f;
            startDampingPosition = "0";
        }
    }

    //First Tutorial imageSet
    void FirstTutorial()
    {
        Invoke("image1_1", 1);
        Invoke("image1_2", 3.5f);
        Invoke("image1_3", 6f);
        Invoke("ScaleDownLoadingText", 9.75f);
        Invoke("takeToLevel", 10.5f);
    }

    void image1_1()
    {
        startDampingColor = "1_1";
        startDampingPosition = "1_1";
        newPosition = tImageSets1[0].GetComponent<Transform>().localPosition;
        dest = -255f;
    }
    void image1_2()
    {
        startDampingColor = "1_2";
        startDampingPosition = "1_2";
        newPosition = tImageSets1[1].GetComponent<Transform>().localPosition;
        dest = 0f;
    }
    void image1_3()
    {
        startDampingColor = "1_3";
        startDampingPosition = "1_3";
        newPosition = tImageSets1[2].GetComponent<Transform>().localPosition;
        dest = 250f;
    }
    //Second Tutorial imageSet...

    void ScaleDownLoadingText()
    {
        loadingTextScaleDest = 0;
    }

    //Load Level
    void takeToLevel()
    {
        startDampingColor = "0";
        startDampingPosition = "0";
        SceneManager.LoadScene(playerAmountObj.GetComponent<PlayerAmount>().currentLevelName);
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
